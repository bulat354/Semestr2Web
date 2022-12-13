﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RickAndMortyLibrary.Common;

namespace RickAndMortyLibrary
{
    /// <summary>
    /// Интерфейс для главного экрана.
    /// </summary>
    public interface IMainUI
    {
        Task WaitForCreating(out string ipAddress, out int port, out GameType gameType, out string userName);
        Task WaitForConnecting(out string ipAddress, out int port, out string userName);
    }
}
