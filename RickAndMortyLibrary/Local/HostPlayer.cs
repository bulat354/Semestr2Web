using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.ErrorMessages;
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

        private IPlayerUI ui;

        public HostPlayer(string userName, IPlayerUI ui)
        {
            UserName = userName;
            this.ui = ui;
        }

        public Task GetReady()
        {
            throw new NotImplementedException();
            
        }

        public void SendError(Error error)
        {
            throw new NotImplementedException();
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
