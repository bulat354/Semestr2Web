using DPTPLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Test
{
    public class Client
    {
        private DPTPClient _client;

        public int Id { get; set; }
        public bool Connected { get => _client.IsConnected; }

        public Client(DPTPClient client)
        {
            _client = client;
        }

        public Client(string ip, int port)
        {
            _client = new DPTPClient(ip, port);
        }

        public StringMessage? WaitForAny()
        {
            try
            {
                while (true)
                {
                    var packet = _client.ReceivePacket();
                    if (packet != null)
                    {
                        var msg = StringMessage.Parse(packet);
                        if (msg != null)
                        {
                            return msg;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public void SendMessage(StringMessage message)
        {
            try
            {
                var packet = message.ToPacket();
                _client.SendPacket(packet);
            }
            catch { return; }
        }
    }
}
