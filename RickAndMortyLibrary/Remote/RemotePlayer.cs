using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.ErrorMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Remote
{
    internal class RemotePlayer : IPlayer
    {
        public string UserName { get; }
        private IClient client;
        public TaskCompletionSource Disconnected { get; }

        public RemotePlayer(IClient client)
        {
            this.client = client;

            Disconnected = new TaskCompletionSource();
        }

        public void SendError(Error error)
        {
        }

        public void ToWaitScreen()
        {

        }

        public void Disconnect()
        {
        }

        public void SendNewPlayer(IPlayer player)
        {
            throw new NotImplementedException();
        }

        public void SendRemovePlayer(IPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
