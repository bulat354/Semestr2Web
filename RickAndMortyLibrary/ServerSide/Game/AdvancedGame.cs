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
                        x.Win();
                    else
                        x.Lose();
                });
            }
        }

        protected bool IsPlayerParasite(IPlayer player)
        {
            return player.GetPerson() == Person.Parasite;
        }

        // начинает фазу обеденного стола, возвращает сторону победивших
        protected async Task<Person> StartDiningTable()
        {
            throw new NotImplementedException();
        }
    }
}
