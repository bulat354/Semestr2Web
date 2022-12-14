using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common.Game.Cards
{
    /// <summary>
    /// Класс для карт персонажей.
    /// </summary>
    public abstract class CharacterCard : Card
    {
        public static string BackImagePath { get; }

        public CharacterCard(string name, string imagePath, CardColor color, int count)
            : base(name, imagePath, color, 1)
        {
        }
    }
}
