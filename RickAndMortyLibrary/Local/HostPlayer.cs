﻿using RickAndMortyLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Local
{
    /// <summary>
    /// Игрок-хост
    /// </summary>
    internal class HostPlayer : IPlayer
    {
        public string UserName { get; }
    }
}
