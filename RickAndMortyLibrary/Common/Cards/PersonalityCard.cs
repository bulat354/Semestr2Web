using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common
{
    /// <summary>
    /// Класс для карт личностей
    /// </summary>
    public class PersonalityCard : Card
    {
        public static string BackImagePath { get; }

        public readonly Person Person;

        public PersonalityCard(string name, string imagePath) : base(name, imagePath, CardColor.None, 1)
        {
        }
    }
}
