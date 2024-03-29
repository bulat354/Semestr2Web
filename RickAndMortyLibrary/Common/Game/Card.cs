﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common.Game
{
    /// <summary>
    /// Базовый класс для карт
    /// </summary>
    public abstract class Card
    {
        public readonly int Id;

        public readonly string Name;
        public readonly string ImagePath;
        public readonly CardColor Color;
        public readonly int InPackCount;

        public Card
            (
            int id, string name, string imagePath, CardColor color,
            int inPackCount
            )
        {
            Id = id;

            Name = name;
            ImagePath = imagePath;
            Color = color;
            InPackCount = inPackCount;
        }
    }
}
