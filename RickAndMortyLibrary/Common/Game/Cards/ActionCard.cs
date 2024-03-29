﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common.Game.Cards
{
    /// <summary>
    /// Класс для карт действий. Игроки ходят ими.
    /// </summary>
    public class ActionCard : Card
    {
        public static string BackImagePath { get; }

        public ActionCard(int id, string name, string imagePath, CardColor color, int count)
            : base(id, name, imagePath, color, count)
        {
        }
    }
}
