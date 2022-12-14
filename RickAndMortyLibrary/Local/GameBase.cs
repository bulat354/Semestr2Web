using RickAndMortyLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Local
{
    /// <summary>
    /// Класс содержащий игровую логику. Находится у хоста.
    /// </summary>
    internal abstract class GameBase
    {
        public TaskCompletionSource GameOverEvent = new TaskCompletionSource();

        private IPlayer[] _players;

        private ICardsPack<ActionCard> actionCardsPack;
        private ICardsPack<CharacterCard> characterCardsPack;
        private ICardsPack<PersonalityCard> personalityCardsPack;

        private List<ActionCard> discardPile;

        private List<Character> characters;

        public void Init(params IPlayer[] players)
        {
            if (players == null)
                throw new ArgumentNullException();

            _players = players;
        }

        public abstract void StartGame();

        internal abstract void StartRound();
        internal abstract void StartFinalRound();

        internal abstract void StartTurn();

        internal abstract void ReactToKill(Character character);
        internal abstract void Invoke(ActionCard action);
    }
}
