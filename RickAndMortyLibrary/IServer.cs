using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTPLibrary;

namespace RickAndMortyLibrary
{
    /// <summary>
    /// Используется для создания хоста.
    /// </summary>
    public interface IServer
    {
        void Initialize(IMainUI ui);

        void Start();

        Task WaitForPlayers();

        void StartGame();
        void CloseGame();

        void Close();
    }
}
