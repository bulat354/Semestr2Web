using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ServerSide;
using RickAndMortyLibrary.Test;
using RickAndMortyUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private List<Tuple<CharacterCard, PersonalityCard, CharacterTag>> table;

        /// <summary>
        /// Starts game and awaits for end
        /// </summary>
        public async Task Start()
        {
            actionsPack = CardsImporter.GetActionCardsPack();
            charactersPack = CardsImporter.GetCharacterCardsPack();
            personsPack = CardsImporter.GetPersonalityCardsPack();

            actionsPack.Shuffle();
            charactersPack.Shuffle();
            personsPack.Shuffle();

            await InitTable();
        }

        private async Task InitTable()
        {
            table = new List<Tuple<CharacterCard, PersonalityCard, CharacterTag>>();

            for (int i = 0; i < PlayerControllers.Length * 2; i++)
            {
                var tuple = Tuple.Create(charactersPack.Pop(), personsPack.Pop(), CharacterTag.None);
                table.Add(tuple);

                await BroadcastMessage(StringMessage.Create(MessageFirstGoal.Character, tuple.Item1.Id.ToString(), MessageSecondGoal.Add));
            }
        }

        /// <summary>
        /// Wait for selecting action cards by players and process these cards
        /// </summary>
        private async Task StartRound()
        {

        }

        /// <summary>
        /// Waits for player actions based on selected action card
        /// </summary>
        /// <returns></returns>
        private async Task StartTurn()
        {

        }

        #region Helper methods
        /// <summary>
        /// Send to all players one message
        /// </summary>
        private async Task BroadcastMessage(StringMessage message)
        {
            foreach (var player in PlayerControllers)
            {
                await player.ProcessMessage(message);
            }
        }
        #endregion
    }
}
