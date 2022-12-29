using RickAndMortyLibrary.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyUI
{
    public class RemotePlayerController : IPlayerController
    {
        private Client _client;

        public int Id { get; set; }

        public RemotePlayerController(Client client, int id)
        {
            _client = client;
            Id = id;
        }

        public StringMessage? ProcessMessage(StringMessage message)
        {
            _client.SendMessage(message);
            return _client.WaitForAny();
        }
    }
}
