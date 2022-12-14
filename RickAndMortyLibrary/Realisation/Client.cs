using DPTPLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary
{
    internal class Client : IClient
    {
        private DPTPClient client;
        public Client(DPTPClient client)
        {
            this.client = client;
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Initialize(IMainUI ui)
        {
            throw new NotImplementedException();
        }

        public Task<object> Receive()
        {
            throw new NotImplementedException();
        }

        public Task Send(object message)
        {
            throw new NotImplementedException();
        }
    }
}
