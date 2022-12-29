using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ServerSide;
using RickAndMortyLibrary.Test;
using RickAndMortyLibrary;
using RickAndMortyUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace RickAndMortyUI
{
    public class GameController
    {
        public bool IsAdvancedMode { get; set; } = false;

        public GameVM Game { get; set; }
        public IPlayerController[] PlayerControllers { get; set; }

        private ICardsPack<ActionCard> actionsPack;
        private ICardsPack<CharacterCard> charactersPack;
        private ICardsPack<PersonalityCard> personsPack;

        private List<Character> killedCharacters;
        private List<ActionCard> discardPile;

        private int fails;
        private Person failPerson;

        private bool isGameOver = false;

        private List<Character> allCharacters = new List<Character>();
        private int round = 1;
        private int firstPlayer => round % PlayerControllers.Length;
        private bool[] actionAttached;

        /// <summary>
        /// Starts game and awaits for end
        /// </summary>
        public void Start()
        {
            PlayerControllers.Shuffle();

            actionsPack = CardsImporter.GetActionCardsPack();
            charactersPack = CardsImporter.GetCharacterCardsPack();
            personsPack = CardsImporter.GetPersonalityCardsPack();

            actionAttached = new bool[PlayerControllers.Length];

            actionsPack.Shuffle();
            charactersPack.Shuffle();
            personsPack.Shuffle();

            InitCharacters();
            if (IsAdvancedMode)
                InitPlayerCharacters();
            InitPlayerHands();

            while (true)
            {
                if (!StartRound(false) || isGameOver)
                    break;
            }

            if (!isGameOver && allCharacters.Any(x => x.Player == null))
                StartRound(true);

            EndGame();
        }

        private void EndGame()
        {
            if (fails >= 4)
            {
                RevealCharacters();
                BroadcastLoseAndWin(x => GetPlayerPerson(x) == failPerson);
            }

            else if (allCharacters.Any(x => x.Player == null && x.Person == Person.Parasite))
            {
                RevealCharacters();
                BroadcastLoseAndWin(x => GetPlayerPerson(x) == Person.Friend);
            }

            else if (IsAdvancedMode)
            {
                GoToDinnerTable();
            }

            else
            {
                RevealCharacters();
                BroadcastLoseAndWin(x => GetPlayerPerson(x) == Person.Parasite);
            }
        }

        private void GoToDinnerTable()
        {
            allCharacters = allCharacters.Where(x => x.Player != null).ToList();

            foreach (var player in PlayerControllers.Where(x => allCharacters.All(x => x.Player != x)))
            {
                Broadcast(new StringMessage(MessageFirstGoal.Person, $"1 {player.Id}", MessageSecondGoal.Attach));
            }

            failPerson = allCharacters.Any(x => x.Person == Person.Parasite) ? Person.Friend : Person.Parasite;

            foreach (var player in PlayerControllers.Where(x => allCharacters.Any(x => x.Player == x)))
            {
                if (GetVoteResult())
                {
                    break;
                }

                var character = WaitForSelectCharacter(player, x => x.Player != player);
                if (character != null)
                {
                    allCharacters.Remove(character);
                    Broadcast(new StringMessage(MessageFirstGoal.Person, $"{character.PersonId} {character.PlayerId}", MessageSecondGoal.Attach));
                    if (character.Person == Person.Friend)
                    {
                        Broadcast(GetPlayerName(player.Id) + $" убил друга!");
                        failPerson = GetPlayerPerson(player);
                        break;
                    }
                }    
            }

            RevealCharacters();
            BroadcastLoseAndWin(x => GetPlayerPerson(x) == failPerson);
        }

        private void InitCharacters()
        {
            for (int i = 0; i < PlayerControllers.Length * 2; i++)
            {
                AddCharacterToTable();
            }
        }

        private void InitPlayerCharacters()
        {
            var cardsPack = CardsImporter.GetPlayerPersonalityCardsPack();
            for (int i = 0; i < PlayerControllers.Length; i++)
            {
                var character = GetNextCharacter();

                character.Player = PlayerControllers[i];
                character.Personality = cardsPack.Pop();

                AttachCharacterToPlayer(character);
            }
        }

        private void InitPlayerHands()
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (var player in PlayerControllers)
                {
                    GiveCardToPlayer(player);
                }
            }
        }

        private bool StartRound(bool isLast)
        {
            if (GetVoteResult())
            {
                isGameOver = true;
                return true;
            }

            discardPile = new List<ActionCard>();
            killedCharacters = new List<Character>();

            Broadcast(isLast ? "Последний раунд" : $"Раунд {round}");
            Broadcast(new StringMessage(MessageFirstGoal.Round, PlayerControllers[firstPlayer].Id.ToString()));//first player id
            var cards = WaitForActions();
            for (int i = 0; i < cards.Length; i++)
                if (cards[i] != null)
                    Broadcast(new StringMessage(MessageFirstGoal.Action, $"{PlayerControllers[i].Id} {cards[i].Id}", MessageSecondGoal.Show));//reveal selected cards
            if (!isLast)
                foreach (var player in PlayerControllers)
                    GiveCardToPlayer(player);

            var result = true;

            for (int i = 0; i < PlayerControllers.Length; i++)
            {
                var index = (firstPlayer + i) % PlayerControllers.Length;
                if (cards[index] == null)
                    continue;

                result &= StartTurn(PlayerControllers[index], cards[index]);
            }
            round++;

            Broadcast(new StringMessage(MessageFirstGoal.Action, null, MessageSecondGoal.Remove));

            if (!isLast)
                if (!AddCharacterToTable())
                    return false;

            if (killedCharacters.Count == 0 && discardPile.Any(x => x.Id == 0))
                foreach (var player in PlayerControllers)
                    GiveCardToPlayer(player);

            return true;
        }

        private bool StartTurn(IPlayerController player, ActionCard card)
        {
            Broadcast(new StringMessage(MessageFirstGoal.Turn, player.Id.ToString()));
            var res = ProcessAction(player, card);
            discardPile.Add(card);
            return res;
        }

        private bool ProcessAction(IPlayerController player, ActionCard card)
        {
            Broadcast(new StringMessage(MessageFirstGoal.Action, $"{player.Id} {card.Id}", MessageSecondGoal.Show));
            Broadcast($"{GetPlayerName(player.Id)} ходит");

            switch (card.Id)
            {
                case 1:
                    return ProcessImToo(player);
                case 2:
                    return ProcessKillSummer(player);
                case 3:
                case 4:
                case 5:
                    Process15Years(player, card); break;
                case 6:
                case 7:
                case 8:
                    ProcessJarry(player, card); break;
                case 9:
                case 10:
                case 11:
                    ProcessMorty(player, card); break;
                case 12:
                case 13:
                case 14:
                    ProcessSummer(player, card); break;
                case 15:
                    return ProcessSomethingWrong(player, card);
                case 16:
                    return ProcessBitchSister(player);
                case 17:
                    return ProcessYouCantKillMe(player);
                case 18:
                case 19:
                case 20:
                    return ProcessBeth(player, card);
                case 21:
                    ProcessRick(player); break;
            }

            return true;
        }

        private void ProcessRick(IPlayerController player)
        {
            if (personsPack.Count > 0)
                Send(player, new StringMessage(MessageFirstGoal.Person, personsPack.Peek().Id.ToString(), MessageSecondGoal.Show));
            else
                Send(player, "Колода личностей пуста");
        }

        private bool ProcessBeth(IPlayerController player, ActionCard card)
        {
            Send(player, "Выберите жертву");
            var character = WaitForSelectCharacter(player, x => x.PlayerId != player.Id && IsColorsMatch(x, card.Color) && IsKillable(x, player));
            if (character == null)
                return true;

            return KillCharacter(player, character);
        }

        private bool ProcessYouCantKillMe(IPlayerController player)
        {
            Send(player, "Выберите другого игрока");
            var another = WaitForSelectPlayer(player);

            Send(another, "Вы должны выбрать жертву");
            var character = WaitForSelectCharacter(another, x => x.PlayerId != another.Id && IsKillable(x, player));
            if (character == null)
                return true;

            return KillCharacter(another, character);
        }

        private bool ProcessBitchSister(IPlayerController player)
        {
            Send(player, "Выберите первую жертву");
            var character1 = WaitForSelectCharacter(player, x => x.PlayerId != player.Id && IsKillable(x, player));
            if (character1 == null)
                return true;

            var result = KillCharacter(player, character1);
            if (isGameOver)
                return true;

            Send(player, "Выберите жертву того же цвета");
            var character2 = WaitForSelectCharacter(player, x => x.PlayerId != player.Id && IsColorsMatch(x, character1.Color) && IsKillable(x, player));
            if (character2 == null)
                return result;

            result &= KillCharacter(player, character2);
            return result;
        }

        private bool ProcessSomethingWrong(IPlayerController player, ActionCard card)
        {
            var index = PlayerControllers.FirstIndex(x => x == player);
            if (actionAttached[index])
            {
                Send(player, "Выберите жертву");
                var character = WaitForSelectCharacter(player, x => x.PlayerId != player.Id && IsKillable(x, player));
                if (character == null)
                    return true;
                actionAttached[index] = false;

                return KillCharacter(player, character);
            }
            else
            {
                Send(player, "Увидимся на следующем раунде");
                actionAttached[index] = true;
                player.ProcessMessage(new StringMessage(MessageFirstGoal.Action, card.Id.ToString(), MessageSecondGoal.Attach));
            }

            return true;
        }

        private void ProcessSummer(IPlayerController player, ActionCard card)
        {
            Send(player, "Раскройте личность двух персонажей");
            var character1 = WaitForSelectCharacter(player, x => x.Color == card.Color && x.PlayerId != player.Id);
            if (character1 == null)
                return;
            player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, $"{character1.CardId} {character1.PersonId}", MessageSecondGoal.Add));

            var character2 = WaitForSelectCharacter(player, x => x.Color == card.Color && x.PlayerId != player.Id && x != character1);
            if (character2 == null) 
                return;
            player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, $"{character2.CardId} {character2.PersonId}", MessageSecondGoal.Add));

            ShuffleCharacterPersons(new[] { character1, character2 });
        }

        private void ProcessMorty(IPlayerController player, ActionCard card)
        {
            Send(player, "Раскройте личность персонажа");
            var character = WaitForSelectCharacter(player, x => x.Player == null && x.Color == card.Color);
            if (character == null)
                return;

            player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, $"{character.CardId} {character.PersonId}", MessageSecondGoal.Add));
        }

        private void ProcessJarry(IPlayerController player, ActionCard card)
        {
            var characters = allCharacters
                .Where(x => x.Color == card.Color)
                .ToArray();
            ShuffleCharacterPersons(characters);

            var another = WaitForSelectPlayer(player);
            GiveCardToPlayer(another);
        }

        private void Process15Years(IPlayerController player, ActionCard card)
        {
            Send(player, "Раскройте личность персонажа навсегда");
            var character = WaitForSelectCharacter(player, x => x.Player == null && x.Color == card.Color);
            if (character == null)
                return;

            character.Tag |= CharacterTag.Immutable;
            player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, $"{character.CardId} {character.PersonId}", MessageSecondGoal.Add));
        }

        private bool ProcessKillSummer(IPlayerController player)
        {
            Send(player, "Выберите жертву");
            var character = WaitForSelectCharacter(player, x => x.Player != player && IsKillable(x, player));
            if (character == null)
                return true;

            return KillCharacter(player, character, false);
        }

        private bool ProcessImToo(IPlayerController player)
        {
            if (discardPile.Count > 0)
                return ProcessAction(player, discardPile[discardPile.Count - 1]);

            return true;
        }

        #region Helper methods
        private Character? WaitForSelectCharacter(IPlayerController player, Func<Character, bool> predicate)
        {
            Broadcast($"{GetPlayerName(player.Id)} выбирает персонажа");

            var characters = this.allCharacters.Where(predicate);
            if (!characters.Any())
                return null;

            var message = player.ProcessMessage(
                new StringMessage(MessageFirstGoal.Character, string.Join(' ', characters.Select(x => x.CardId.ToString()))));
            if (message == null)
                return null;

            var id = message.ToInt();
            return characters.FirstOrDefault(x => x.CardId == id);
        }

        private ActionCard?[] WaitForActions()
        {
            Broadcast("Выберите действие");
            var message = new StringMessage(MessageFirstGoal.Action);

            return WaitAll(message)
                .Select(x => x == null ? null : CardsImporter.GetCard<ActionCard>(x.ToInt()))
                .ToArray();
        }

        private IPlayerController WaitForSelectPlayer(IPlayerController player)
        {
            Broadcast($"{GetPlayerName(player.Id)} выбирает игрока");
            var message = player.ProcessMessage(new StringMessage(MessageFirstGoal.Player, null));
            return PlayerControllers.First(x => x.Id == message.ToInt());
        }

        private bool KillCharacter(IPlayerController player, Character character, bool hasAftermath = true)
        {
            if (character.CardId == 11 && !character.Tag.HasFlag(CharacterTag.CanKill))
            {
                Broadcast("Чудовище Франкенштейна не просто убить!");
                character.Tag |= CharacterTag.CanKill;
                return true;
            }

            var result = true;
            allCharacters.Remove(character);
            if (character.Player == null)
            {
                if (character.CardId != 19)
                {
                    Broadcast(new StringMessage(MessageFirstGoal.Person, $"{character.CardId} {character.PersonId}", MessageSecondGoal.Add));
                    Broadcast(new StringMessage(MessageFirstGoal.Character, character.CardId.ToString(), MessageSecondGoal.Remove));
                }
            }
            else
            {
                if (character.CardId != 19)
                {
                    Broadcast(new StringMessage(MessageFirstGoal.Person, $"{character.CardId} {character.PersonId}", MessageSecondGoal.Add));
                    Broadcast(new StringMessage(MessageFirstGoal.Character, character.PlayerId.ToString(), MessageSecondGoal.Detach));
                }
                character.Player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, null, MessageSecondGoal.Detach));
            }

            if (character.Person == Person.Parasite)
            {
                if (character.CardId != 19)
                {
                    Broadcast(GetPlayerName(player.Id) + " убил паразита!");
                }
            }
            if (character.Person == Person.Friend)
            {
                fails++;
                failPerson = GetPlayerPerson(player);
                if (character.CardId != 19 && fails < 4)
                {
                    Broadcast(GetPlayerName(player.Id) + $" убил друга! Уже {fails} в гробу!");
                }
                else
                    Broadcast($"Кого убил {GetPlayerName(player.Id)} узнаете через 10 секунд");

                if (character.CardId == 1)
                {
                    Broadcast("Ой-ой.");
                    fails = 4;
                    isGameOver = true;
                }
                if (fails >= 4)
                    isGameOver = true;

                if (isGameOver)
                    return true;

                if (hasAftermath)
                {
                    Send(player, "В наказание сбросьте карту");
                    var message = player.ProcessMessage(new StringMessage(MessageFirstGoal.Action));
                    result &= AddCharacterToTable();
                }

                if (character.Player != null)
                {
                    var newChar = GetNextCharacter();
                    if (newChar == null)
                        result = false;
                    else
                    {
                        newChar.Player = character.Player;
                        AttachCharacterToPlayer(newChar);
                    }
                }
            }

            if (character.CardId == 19)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(10000);
                    Broadcast(new StringMessage(MessageFirstGoal.Person, $"{character.CardId} {character.PersonId}", MessageSecondGoal.Add));
                    if (character.Player == null)
                        Broadcast(new StringMessage(MessageFirstGoal.Character, character.CardId.ToString(), MessageSecondGoal.Remove));
                    else
                        Broadcast(new StringMessage(MessageFirstGoal.Character, character.PlayerId.ToString(), MessageSecondGoal.Detach));
                });
            }
            else if (character.CardId == 0)
            {
                Send(player, "Сыграйте еще одну карту");
                var message = player.ProcessMessage(new StringMessage(MessageFirstGoal.Action));
                if (message != null)
                {
                    var card = CardsImporter.GetCard<ActionCard>(message.ToInt());
                    result &= StartTurn(player, card);
                    GiveCardToPlayer(player);
                }
            }
            else if (character.CardId == 20)
            {
                Send(player, "Подсмотрите личность персонажа");
                var selectedChar = WaitForSelectCharacter(player, x => x.Player != null && x.Player != player);
                if (character != null)
                    player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, $"{character.CardId} {character.PersonId}", MessageSecondGoal.Add));
            }
            else if (character.CardId == 21)
                GiveCardToPlayer(player);

            return result;
        }

        private bool IsColorsMatch(Character character, CardColor color)
        {
            return character.Color == color || character.CardId == 7;
        }

        private bool IsKillable(Character character, IPlayerController player)
        {
            return (character.CardId != 10 || !discardPile.Any(x => x.Id > 5 && x.Id < 9)) 
                && (character.CardId != 15 || player == PlayerControllers[firstPlayer]);
        }

        private void GiveCardToPlayer(IPlayerController player)
        {
            var card = actionsPack.Pop();
            player.ProcessMessage(new StringMessage(MessageFirstGoal.Action, card.Id.ToString(), MessageSecondGoal.Add));
        }

        private void AttachCharacterToPlayer(Character character)
        {
            Broadcast($"{GetPlayerName(character.PlayerId)} нашел нового друга");
            allCharacters.Add(character);
            Broadcast(new StringMessage(MessageFirstGoal.Character, $"{character.CardId} {character.PlayerId}", MessageSecondGoal.Attach));
            character.Player.ProcessMessage(new StringMessage(MessageFirstGoal.Person, character.PersonId.ToString(), MessageSecondGoal.Attach));
        }

        private bool AddCharacterToTable()
        {
            var character = GetNextCharacter();
            if (character == null)
                return false;

            allCharacters.Add(character);
            Broadcast(new StringMessage(MessageFirstGoal.Character, character.CardId.ToString(), MessageSecondGoal.Add));
            Broadcast("Приветствуем гостя " + character.Card.Name);

            return true;
        }

        private Character? GetNextCharacter()
        {
            if (charactersPack.Count == 0)
                return null;

            var card1 = charactersPack.Pop();
            var card2 = personsPack.Pop();

            return new Character()
            {
                Card = card1,
                Personality = card2,
                Tag = CharacterTag.None
            };
        }

        private void ShuffleCharacterPersons(Character[] characters)
        {
            Broadcast("Перемешиваем личности");
            Broadcast(new StringMessage(MessageFirstGoal.Character, string.Join(" ", characters.Select(x => x.CardId.ToString())), MessageSecondGoal.Shuffle));
            var random = new Random();

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].SwapPersons(characters[random.Next(0, characters.Length)]);
            }
        }

        private bool GetVoteResult()
        {
            Broadcast("Голосование!");
            var messages = WaitAll(new StringMessage(MessageFirstGoal.Voting, null));
            var count = messages.Count(x => x.Message == "yes");
            return count >= PlayerControllers.Length / 2;
        }

        private void Broadcast(string message)
        {
            var msg = StringMessage.Create(MessageFirstGoal.Message, message, MessageSecondGoal.None);
            Broadcast(msg);
        }

        /// <summary>
        /// Send to all players one message
        /// </summary>
        private void Broadcast(StringMessage message)
        {
            var tasks = PlayerControllers
                .Select(x => Task.Run(() =>
                {
                    x.ProcessMessage(message);
                }))
                .ToArray();
            Task.WaitAll(tasks);
        }

        private void Send(IPlayerController player, string message)
        {
            player.ProcessMessage(new StringMessage(MessageFirstGoal.Message, message));
        }

        private void Send(IPlayerController player, StringMessage message)
        {
            player.ProcessMessage(message);
        }

        private void BroadcastLoseAndWin(Func<IPlayerController, bool> predicateForLosers)
        {
            foreach (var player in PlayerControllers)
            {
                if (predicateForLosers(player))
                    player.ProcessMessage(new StringMessage(MessageFirstGoal.GameOver, null, MessageSecondGoal.Lose));
                else
                    player.ProcessMessage(new StringMessage(MessageFirstGoal.GameOver, null, MessageSecondGoal.Win));
            }
        }

        private void RevealCharacters()
        {
            foreach (var character in allCharacters)
            {
                Broadcast(new StringMessage(MessageFirstGoal.Person, $"{character.CardId} {character.PersonId}", MessageSecondGoal.Add));
            }
        }

        private StringMessage?[] WaitAll(StringMessage message)
        {
            var tasks = PlayerControllers
                .Select(x => Task.Run(() => x.ProcessMessage(message)))
                .ToArray();
            Task.WaitAll(tasks);
            return tasks.Select(x => x.Result).ToArray();
        }

        private StringMessage? WaitAny(StringMessage message)
        {
            var tasks = PlayerControllers
                .Select(x => Task.Run(() => x.ProcessMessage(message)))
                .ToArray();
            var index = Task.WaitAny(tasks);
            return tasks[index].Result;
        }

        private string GetPlayerName(int id)
        {
            switch (id)
            {
                case 0:
                    return "Бэт";
                case 1:
                    return "Саммер";
                case 2:
                    return "Рик";
                case 3:
                    return "Морти";
                case 4:
                    return "Джерри";
            }

            return "Игрок";
        }

        private string GetColorName(CardColor color)
        {
            switch (color)
            {
                case CardColor.Red:
                    return "красн";
                case CardColor.Green:
                    return "зелен";
                case CardColor.Blue:
                    return "голуб";
            }

            return "бел";
        }

        private Person GetPlayerPerson(IPlayerController player)
        {
            var character = allCharacters.FirstOrDefault(x => x.Player == player);
            if (character == null)
                return Person.Friend;
            
            return character.Person;
        }
        #endregion
    }
}
