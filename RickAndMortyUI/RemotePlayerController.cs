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

        public async Task<StringMessage?> ProcessMessage(StringMessage message)
        {
            await _client.SendMessage(message);

            var goals = IsRequest(message);
            if (goals != null)
                return await _client.WaitForMessage(goals.Item1, goals.Item2);

            return null;
        }

        public Tuple<MessageFirstGoal, MessageSecondGoal>? IsRequest(StringMessage message)
        {
            return null;
        }
    }
}
