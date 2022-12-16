using RickAndMortyLibrary.Common.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ServerSide
{
    internal class AdvancedGame : NormalGame
    {
        protected Person lastFailer;
        protected Character[] playerCharacters
        {
            get
            {
                return _players.Select(x => x.GetCharacter()).ToArray();
            }
        }

        protected override void LayCharacters()
        {
            base.LayCharacters();

            var playerPack = CardsImporter.GetPlayerPersonalityCardsPack();
            playerPack.Shuffle();

            _players.ForEach(x => x.AttachCharacter(new Character()
            {
                Card = characterCardsPack.Pop(),
                Personality = playerPack.Pop(),
                IsAttachedToPlayer = true
            }, x.UserName));
        }

        protected override void CheckForWinners()
        {
            // если убили 4 друзей, то проигрывает команда человека, который убил 4-го
            if (fails == 4)
            {
                _players.ForEach(x =>
                {
                    if (x.GetPerson() == lastFailer)
                        x.Lose();
                    else
                        x.Win();
                });
            }
            // если после финального раунда или после голосования
            // среди персонажей на столе есть паразит, то выигрывают паразиты
            else if (characters.Any(x => IsCharacterParasite(x)))
            {
                _players.ForEach(x =>
                {
                    if (IsPlayerParasite(x))
                        x.Win();
                    else
                        x.Lose();
                });
            }
            // иначе пусть решится все за обеденным столом!
            else
            {
                var task = StartDiningTable();
                task.Wait();
                var winSide = task.Result;

                _players.ForEach(x =>
                {
                    if (x.GetPerson() == winSide)
                        x.Lose();
                    else
                        x.Win();
                });
            }
        }

        protected bool IsPlayerParasite(IPlayer player)
        {
            return player.GetPerson() == Person.Parasite;
        }

        // начинает фазу обеденного стола, возвращает сторону проигравших
        protected async Task<Person> StartDiningTable()
        {
            for (int i = 0; i < _players.Length; i++)
            {
                WaitForVoting();

                var player = _players[i];
                if (playerCharacters[i] == null)
                    continue;

                var character = await player.WaitForSelectCharacter(x => x.IsAttachedToPlayer).WaitAsync(stopGame.Token);
                if (stopGame.IsCancellationRequested)
                    break;

                if (character.Personality.Person == Person.Parasite)
                {
                    continue;
                }
                else
                {
                    stopWaitingVoting.Cancel();
                    return player.GetPerson();
                }
            }

            return _players.Any(x => x.GetPerson() == Person.Parasite) ? Person.Parasite : Person.Friend;
        }

        protected override IEnumerable<Character> GetCharacters()
        {
            return characters.Concat(playerCharacters);
        }

        protected override IEnumerable<Character> GetCharacters(Func<Character, bool> predicate)
        {
            return GetCharacters().Where(predicate);
        }

        protected override async Task KillCharacter(IPlayer player, Character? character, bool toDiscard = true, bool newCharacter = true)
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

            if (!character.IsAttachedToPlayer)
                characters.Remove(character);
            else
            {
                if (character.Personality.Person == Person.Parasite)
                {
                    playerCharacters[player.Number] = null;
                    player.AttachCharacter(null, player.UserName);
                }
                else
                {
                    var newChar = GetNewCharacter();
                    playerCharacters[player.Number] = newChar;
                    player.AttachCharacter(newChar, player.UserName);
                }
            }

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
                lastFailer = player.GetPerson();

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

        protected override async Task InvokeKilling(Character character, IPlayer player)
        {
            await base.InvokeKilling(character, player);

            if (character.Card.Id == 20)
                await InvokeGiraffe(player);
        }

        protected async Task InvokeGiraffe(IPlayer player)
        {
            var character = await WaitForSelectCharacter(player, x => x.IsAttachedToPlayer);

            if (character != null)
                player.ShowCharacterPerson(character);
        }
    }
}
