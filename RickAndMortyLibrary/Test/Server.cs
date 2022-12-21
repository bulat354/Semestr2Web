using DPTPLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Test
{
    public class Server
    {
        private DPTPListener _listener;
        public List<Client> Clients { get; private set; }

        public Server(string ip, int port)
        {
            _listener = new DPTPListener(ip, port);
            _listener.Start();
            Clients = new List<Client>();
        }

        public Server(int port)
        {
            _listener = new DPTPListener(System.Net.IPAddress.Any, port);
            _listener.Start();
            Clients = new List<Client>();
        }

        public async Task<Client> AwaitJoining(CancellationToken token)
        {
            try
            {
                var client = new Client(await _listener.AcceptClientAsync().WaitAsync(token));

                Clients.Add(client);
                return client;
            }
            catch
            {
                return null;
            }
        }

        public async Task BroadcastMessage(StringMessage message)
        {
            try
            {
                foreach (var client in Clients)
                {
                    if (client != null)
                        client.SendMessage(message);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
