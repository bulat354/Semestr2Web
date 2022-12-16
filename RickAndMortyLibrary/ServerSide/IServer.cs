using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTPLibrary;
using RickAndMortyLibrary.Common;

namespace RickAndMortyLibrary.ServerSide
{
    /// <summary>
    /// Используется для создания хоста.
    /// </summary>
    public interface IServer
    {
        Task Start(IMainUI ui);
    }
}
