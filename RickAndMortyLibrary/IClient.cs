using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary
{
    /// <summary>
    /// Класс для связи с клиентом или сервером.
    /// </summary>
    public interface IClient
    {
        void Initialize(IMainUI ui);

        void Connect();

        Task<object> Receive();
        Task Send(object message);

        void Disconnect();
    }
}
