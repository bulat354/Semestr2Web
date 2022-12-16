using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ServerSide
{
    internal class NormalGame : GameBase
    {
        protected CancellationTokenSource stopGame;
        protected CancellationTokenSource stopWaitingVoting;

        protected int fails;

        public async override void StartGame()
        {
            // токен для остановки игры
            stopGame = new CancellationTokenSource();

            // перемешаем карты
            actionCardsPack.Shuffle();
            characterCardsPack.Shuffle();
            personalityCardsPack.Shuffle();

            // раздадим карты действий игрокам
            HandOutActionCards();
            // положим на стол персонажей
            LayCharacters();
            // перемешаем игроков
            ShufflePlayers();

            // на другом потоке запустим ожидание голосования
            WaitForVoting();

            var firstPlayer = 0;
            // пока игру не остановили
            while (stopGame.IsCancellationRequested)
            {
                // запускаем таймер на 15 секунд
                _players.ForEach(x => x.StartTimer(15));
                await Task.Delay(TimeSpan.FromSeconds(15), stopGame.Token);
                // и останавливаем, если игра остановлена
                if (stopGame.IsCancellationRequested)
                {
                    _players.ForEach(x => x.StopTimer());
                    break;
                }

                // если на столе остались персонажи или застрелили меньше четырех друзей
                if (characters.Count > 0 || fails < 4)
                {
                    // начинаем раунд
                    if (characterCardsPack.Count > 0)
                        await StartRound(firstPlayer, false);
                    else
                    {
                        // играем последний раунд и заканчиваем игру, если персонажей не осталось
                        await StartRound(firstPlayer, true);
                        break;
                    }
                }
                else
                    break;

                firstPlayer++;
            }

            stopWaitingVoting.Cancel();
            // объявляем победителей
            CheckForWinners();
        }

        // добавляет по три карты действия игрокам
        protected void HandOutActionCards()
        {
            for (int i = 0; i < 3; i++)
                foreach (var player in _players)
                {
                    player.TakeCard(PopCard());
                }
        }

        // кладет на стол персонажей
        protected virtual void LayCharacters()
        {
            var count = _players.Length * 2;

            for (int i = 0; i < count; i++)
            {
                AddCharacterToTable();
            }
        }

        // кладет на стол нового персонажа
        protected void AddCharacterToTable()
        {
            var character = new Character()
            {
                Card = characterCardsPack.Pop(),
                Personality = personalityCardsPack.Pop()
            };
            // сообщает всем о добавлении персонажа
            _players.ForEach(p => p.AddCharacter(character));
        }

        // перемешивает игроков
        protected void ShufflePlayers()
        {
            var random = new Random();
            for (int i = 0; i < _players.Length; i++)
            {
                var num = random.Next(0, _players.Length);

                var player = _players[i];
                _players[i] = _players[num];
                _players[num] = player;
            }
        }

        // берет карту из колоды и, если карт не осталось, сброс делает колодой и перемешивает
        protected ActionCard PopCard()
        {
            if (actionCardsPack.Count == 0)
            {
                actionCardsPack.Init(discardPile);
                discardPile.Clear();
            }

            return actionCardsPack.Pop();
        }

        // объявляет победителей
        protected virtual void CheckForWinners()
        {
            if (fails == 4 || characters.Any(x => IsCharacterParasite(x)))
            {
                _players.ForEach(x => x.Lose());
            }
            else
            {
                _players.ForEach(x => x.Win());
            }
            GameOverEvent.SetResult();
        }

        // проверяет персонажа на паразита
        protected bool IsCharacterParasite(Character character)
        {
            return character.Personality.Person == Person.Parasite;
        }

        //--------------------------------------------------------

        protected ActionCard?[] roundCards;
        protected Stack<ActionCard> playedCards;

        protected bool IsNobodyKilled;

        // начинает раунд
        protected virtual async Task StartRound(int firstPlayer, bool isFinal = false)
        {
            playedCards = new Stack<ActionCard>();
            IsNobodyKilled = true;
            
            // ждет выбора карты действия для каждого игрока
            var tasks = _players.Select(x => x.WaitChoosingAction(stopGame.Token));
            await Task.WhenAll(tasks);
            if (stopGame.IsCancellationRequested)
                return;

            // берет их карты
            roundCards = tasks.Select(x => x.Result).ToArray();
            if (!isFinal)
                // добавляет новые карты
                _players.ForEach(x => x.TakeCard(PopCard()));

            for (int i = 0; i < _players.Length; i++)
            {
                if (stopGame.IsCancellationRequested)
                    return;

                // выполняет действие для каждой карты
                var number = (firstPlayer + i) % _players.Length;
                var card = roundCards[number];

                await Invoke(card, _players[number]).WaitAsync(stopGame.Token);

                if (stopGame.IsCancellationRequested)
                    return;
            }

            if (!isFinal)
                AddCharacterToTable();

            if (!isFinal && IsNobodyKilled && playedCards.Any(x => x.Id == 0))
                _players.ForEach(x => x.TakeCard(PopCard()));
        }

        // ожидает голосования
        protected async Task WaitForVoting()
        {
            while (true)
            {
                if (stopWaitingVoting == null)
                    stopWaitingVoting = new CancellationTokenSource();
                else
                    stopWaitingVoting.TryReset();

                // ждет пока один из игроков не нажмет на кнопку голосования
                await Task.WhenAny(_players.Select(x => x.WaitForVote(stopWaitingVoting.Token)));
                stopWaitingVoting.Cancel(false);

                // начинаем голосовать
                _players.ForEach(p => p.StartVoting());

                // голосуем и подсчитываем количество голосов
                var tasks = _players.Select(x => x.WaitVoteResult());
                var count = (await Task.WhenAll(tasks)).Where(x => x).Count();

                // останавливаем голосование
                _players.ForEach(x => x.StopVoting());

                // если голосов больше половины
                if (count > _players.Length / 2)
                {
                    // останавливаем игру
                    stopGame.Cancel(false);
                    break;
                }
            }
        }

        protected virtual Character? GetTheSame(Character? character)
        {
            if (character == null)
                return null;
            var id = character.Card.Id;

            return characters.FirstOrDefault(x => x.Card.Id == id);
        }

        // выполняет действие из карты действия
        protected async Task Invoke(ActionCard? action, IPlayer player)
        {
            if (action == null)
                return;

            switch (action.Id)
            {
                case (0):
                    await InvokeMrPoopybutthole();
                    break;
                case (1):
                    await InvokeImToo(player);
                    break;
                case (2):
                    await InvokeKillSummer(player);
                    break;
                case (15):
                    await InvokeIsSomethingWrong(player, action);
                    break;
                case (16):
                    await InvokeBitch(player);
                    break;
                case (17):
                    await InvokeYouCantDoThis(player);
                    break;
                case (21):
                    await InvokeRick(player);
                    break;
            }

            if (action.Id < 6)
                await Invoke15Years(action.Color, player);
            else if (action.Id < 9)
                await InvokeJarry(action.Color, player);
            else if (action.Id < 12)
                await InvokeMorty(action.Color, player);
            else if (action.Id < 15)
                await InvokeSummer(action.Color, player);
            else if (action.Id < 21)
                await InvokeBeth(action.Color, player);

            // карту в сброс
            if (card != null)
            {
                discardPile.Add(card.Id);
                playedCards.Push(card);
            }
        }

        #region Methods for working with cards
        protected virtual async Task ShuffleCharacterPersons(Func<Character, bool> predicate)
        {
            var toShuffle = characters
                .Where(x => predicate(x) && !x.Immutable)
                .ToArray();

            var random = new Random();
            for (int i = 0; i < toShuffle.Length; i++)
            {
                SwapPersons(toShuffle[i], toShuffle[random.Next()]);
            }
        }

        protected void SwapPersons(Character one, Character two)
        {
            var person = one.Personality;
            one.Personality = two.Personality;
            two.Personality = person;
        }

        protected virtual async Task KillCharacter(IPlayer player, Character? character, bool toDiscard = true, bool newCharacter = true)
        {
            if (character == null)
                return;

            if (character.Card.Id == 11 && !character.IsKillable)
            {
                character.IsKillable = true;
                return;
            }
            else if (character.Card.Id == 10 && playedCards.Any(x => x.Id >= 6 && x.Id <= 8))
            {
                return;
            }
            else if (character.Card.Id == 15 && playedCards.Count > 0)
            {
                return;
            }

            characters.Remove(character);
            if (character.Card.Id == 19)
                _players.ForEach(x => x.RemoveCharacter(character, 15));
            else
                _players.ForEach(x => x.RemoveCharacter(character));

            if (character.Personality.Person == Person.Friend)
            {
                if (character.Card.Id == 1)
                    fails = 4;
                else
                    fails++;
                _players.ForEach(x => x.PlayerFailed(player));

                if (toDiscard)
                {
                    var action = await player.WaitChoosingAction(stopGame.Token);
                    discardPile.Add(action.Id);
                }    

                if (newCharacter)
                {
                    AddCharacterToTable();
                }
            }

            await InvokeKilling(character, player);
        }

        protected virtual bool AnyCharacter(Func<Character, bool> predicate)
        {
            return characters.Any(x => predicate(x));
        }

        protected virtual CardColor[] GetGameColors()
        {
            var colors = new HashSet<CardColor>();
            foreach (var character in characters)
            {
                if (!colors.Contains(character.Card.Color))
                    colors.Add(character.Card.Color);

                if (colors.Count == 3)
                    break;
            }

            return colors.ToArray();
        }

        protected virtual async Task<Character> WaitForSelectCharacter(IPlayer player, Func<Character, bool> predicate)
        {
            if (!AnyCharacter(predicate))
                return null;

            var character = await player.WaitForSelectCharacter(predicate);

            return GetTheSame(character);
        }

        protected virtual async Task InvokeActionFromHand(IPlayer player)
        {
            var action = await player.WaitChoosingAction(stopGame.Token);
            await Invoke(action, player);
        }
        #endregion

        #region Action Cards
        protected virtual async Task InvokeJarry(CardColor color, IPlayer player)
        {
            await ShuffleCharacterPersons(x => x.Card.Color == color);

            var userName = await player.WaitForSelectPlayer();
            var another = _players.FirstOrDefault(x => x.UserName == userName);

            if (another != null)
            {
                another.TakeCard(PopCard());
            }
        }

        protected virtual async Task InvokeBeth(CardColor color, IPlayer player)
        {
            var character = await WaitForSelectCharacter(player, x => x.Card.Color == color);

            await KillCharacter(player, character);
        }

        protected virtual async Task InvokeKillSummer(IPlayer player)
        {
            var character = await WaitForSelectCharacter(player, x => true);

            await KillCharacter(player, character, false, false);
        }

        protected virtual async Task InvokeImToo(IPlayer player)
        {
            await Invoke(playedCards.Peek(), player);
        }

        protected virtual async Task InvokeMrPoopybutthole()
        { }

        protected virtual async Task Invoke15Years(CardColor color, IPlayer player)
        {
            var character = await WaitForSelectCharacter(player, x => !x.IsAttachedToPlayer && x.Card.Color == color);

            if (character != null)
            {
                player.ShowCharacterPerson(character);
                character.Immutable = true;
            }
        }

        protected virtual async Task InvokeRick(IPlayer player)
        {
            player.ShowTopFromPack(personalityCardsPack.Peek());
        }

        protected virtual async Task InvokeBitch(IPlayer player)
        {
            var color = await player.WaitForSelectColor(GetGameColors());

            for (int i = 0; i < 2; i++)
            {
                var character = await WaitForSelectCharacter(player, x => x.Card.Color == color);

                await KillCharacter(player, character);
            }
        }

        protected virtual async Task InvokeIsSomethingWrong(IPlayer player, ActionCard action)
        {
            player.AttachNextActionCard(action);
        }

        protected virtual async Task InvokeYouCantDoThis(IPlayer player)
        {
            var userName = await player.WaitForSelectPlayer();
            var another = _players.FirstOrDefault(x => x.UserName == userName);

            if (another != null)
            {
                var character = await WaitForSelectCharacter(player, x => true);

                await KillCharacter(player, character);
            }
        }

        protected virtual async Task InvokeMorty(CardColor color, IPlayer player)
        {
            var character = await WaitForSelectCharacter(player, x => x.Card.Color == color && !x.IsAttachedToPlayer);

            if (character != null)
                player.ShowCharacterPerson(character);
        }

        protected virtual async Task InvokeSummer(CardColor color, IPlayer player)
        {
            var character1 = await WaitForSelectCharacter(player, x => x.Card.Color == color);

            if (character1 != null)
                player.ShowCharacterPerson(character1);

            var character2 = await WaitForSelectCharacter(player, x => x.Card.Color == color && x != character1);

            if (character2 != null)
                player.ShowCharacterPerson(character2);

            await ShuffleCharacterPersons(x => x == character1 || x == character2);
        }
        #endregion

        #region Character Cards
        protected virtual async Task InvokeKilling(Character character, IPlayer player)
        {
            var id = character.Card.Id;

            switch (id)
            {
                case 0:
                    await InvokeBoregar(player);
                    break;
                case 21:
                    await InvokeSteve(player);
                    break;
            }
        }

        protected virtual async Task InvokeBoregar(IPlayer player)
        {
            await InvokeActionFromHand(player);
            player.TakeCard(PopCard());
        }

        protected virtual async Task InvokeSteve(IPlayer player)
        {
            player.TakeCard(PopCard());
        }
        #endregion
    }
}
