﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common
{
    /// <summary>
    /// Класс для карт действий. Игроки ходят ими.
    /// </summary>
    public abstract class ActionCard : Card
    {
        public static string BackImagePath { get; }

        public ActionCard(string name, string imagePath, CardColor color, int count) 
            : base(name, imagePath, color, count)
        {
        }
    }
}
