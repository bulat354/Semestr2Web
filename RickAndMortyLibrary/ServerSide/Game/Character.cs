using RickAndMortyLibrary.Common.Game.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ServerSide
{
    /// <summary>
    /// Класс для персонажа. Он содержит карты персонажа и личности.
    /// </summary>
    public class Character
    {
        public CharacterCard Card { get; internal set; }
        public PersonalityCard Personality { get; internal set; }
    }
}
