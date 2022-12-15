using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RickAndMortyLibrary.Common.Game;

namespace RickAndMortyLibrary.ServerSide
{
    /// <summary>
    /// Колода карт.
    /// </summary>
    /// <typeparam name="T">Тип карт</typeparam>
    internal interface ICardsPack<T> where T : Card
    {
        int Count { get; }

        /// <summary>
        /// Тасует карты
        /// </summary>
        void Shuffle();

        /// <summary>
        /// Удаляет карту из колоды
        /// </summary>
        void Remove(int cardId);
        /// <summary>
        /// Добавляет карту в колоду
        /// </summary>
        void Add(int cardId);

        /// <summary>
        /// Берет верхнюю карту и удаляет из колоды
        /// </summary>
        T Pop();
        /// <summary>
        /// Берет верхнюю карту не удаляя ее (подсматривает)
        /// </summary>
        T Peek();

        ICardsPack<T> Init(IEnumerable<int> cards);
    }
}
