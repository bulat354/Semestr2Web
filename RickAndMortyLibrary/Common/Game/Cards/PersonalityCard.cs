﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common.Game.Cards
{
    /// <summary>
    /// Класс для карт личностей
    /// </summary>
    public class PersonalityCard : Card
    {
        public static string BackImagePath { get; }

        public readonly Person Person;

        public PersonalityCard(string name, string imagePath, int count) : base(name, imagePath, CardColor.None, count)
        {
        }
    }
}
