using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.Common.Game;

namespace RickAndMortyLibrary.ServerSide
{
    internal class CardsPack<T> : ICardsPack<T>
        where T : Card
    {
        private List<int> _cards;

        public int Count { get { return _cards.Count; } }

        public CardsPack()
        {
            _cards = new List<int>();
        }

        private void Swap(int index1, int index2)
        {
            var temp = _cards[index1];
            _cards[index1] = _cards[index2];
            _cards[index2] = temp;
        }

        public void Shuffle()
        {
            int newIndex;
            var random = new Random();
            for (int n = _cards.Count - 1; n > 0; n--)
            {
                newIndex = random.Next(n);
                Swap(n, newIndex);
            }
        }

        public void Remove(int cardId) => _cards.Remove(cardId);

        public void Add(int cardId) => _cards.Add(cardId);

        public T Pop()
        {
            var topCard = _cards[0];
            _cards.RemoveAt(0);
            return CardsImporter.GetCard<T>(topCard);
        }

        public T Peek()
        {
            return CardsImporter.GetCard<T>(_cards[0]);
        }

        public ICardsPack<T> Init(IEnumerable<int> cards)
        {
            _cards = cards.ToList();
            return this;
        }
    }
}