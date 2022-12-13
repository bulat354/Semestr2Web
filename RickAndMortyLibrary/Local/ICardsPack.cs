using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RickAndMortyLibrary.Common;

namespace RickAndMortyLibrary.Local
{
    /// <summary>
    /// Колода карт.
    /// </summary>
    /// <typeparam name="T">Тип карт</typeparam>
    internal interface ICardsPack<T> where T : Card
    {
        /// <summary>
        /// Тасует карты
        /// </summary>
        void Shuffle();

        /// <summary>
        /// Удаляет карту из колоды
        /// </summary>
        void Remove(T card);
        /// <summary>
        /// Добавляет карту в колоду
        /// </summary>
        void Add(T card);

        /// <summary>
        /// Берет верхнюю карту и удаляет из колоды
        /// </summary>
        T Pop();
        /// <summary>
        /// Берет верхнюю карту не удаляя ее (подсматривает)
        /// </summary>
        T Peek();

        ICardsPack<T> Init(IEnumerable<T> cards);
    }
}
