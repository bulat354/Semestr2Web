using RickAndMortyLibrary.ErrorMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common
{
    /// <summary>
    /// Интерфейс игрока. Используется для ожидания действий игрока и отправки результатов действий игроков.
    /// </summary>
    internal interface IPlayer
    {
        string UserName { get; }

        Task GetReady();
        void SendError(Error error);

        void SendNewPlayer(IPlayer player);
        void SendRemovePlayer(IPlayer player);
    }
}
