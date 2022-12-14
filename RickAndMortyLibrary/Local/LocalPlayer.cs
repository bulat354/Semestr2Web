using RickAndMortyLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Local
{
    internal class LocalPlayer : IPlayer
    {
        public string UserName { get; set; }

        public Task GetReady()
        {
            throw new NotImplementedException();
        }
    }
}
