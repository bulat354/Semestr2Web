using RickAndMortyLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common
{
    /// <summary>
    /// Класс для связи с клиентом или сервером.
    /// </summary>
    public interface IClient
    {
        void Connect(string ip, int port);

        Task<IMessage> Receive();
        void Send(IMessage message);

        void Disconnect();
    }
}
