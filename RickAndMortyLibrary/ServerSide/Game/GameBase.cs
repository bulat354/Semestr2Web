using RickAndMortyLibrary.Common.Game.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ServerSide
{
    /// <summary>
    /// Класс содержащий игровую логику. Находится у хоста.
    /// </summary>
    internal abstract class GameBase
    {
        public TaskCompletionSource GameOverEvent = new TaskCompletionSource();

        protected IPlayer[] _players;

        protected ICardsPack<ActionCard> actionCardsPack;
        protected ICardsPack<CharacterCard> characterCardsPack;
        protected ICardsPack<PersonalityCard> personalityCardsPack;

        protected List<ActionCard> discardPile;

        protected List<Character> characters;

        public void Init(params IPlayer[] players)
        {
            if (players == null)
                throw new ArgumentNullException();

            _players = players;

            actionCardsPack = CardsImporter.GetActionCardsPack();
            characterCardsPack = CardsImporter.GetCharacterCardsPack();
            personalityCardsPack = CardsImporter.GetPersonalityCardsPack();

            discardPile = new List<ActionCard>();
            characters = new List<Character>();
        }

        public abstract void StartGame();
    }
}
