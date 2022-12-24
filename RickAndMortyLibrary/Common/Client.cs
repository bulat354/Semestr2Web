using DPTPLibrary;
using RickAndMortyLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common
{
    internal class Client : IClient
    {
        private DPTPClient client;
        public bool IsConnected
        {
            get { return client != null && client.IsConnected; }
        }

        public Client() { }
        public Client(DPTPClient client)
        {
            this.client = client;
        }

        public void Connect(string ip, int port)
        {
            client = new DPTPClient(ip, port);
        }

        public void Disconnect()
        {
            client.Close();
        }

        public async Task<IMessage> Receive()
        {
            var packet = await client.ReceivePacket();
            return MessageParser.Parse(packet);
        }

        public async void Send(IMessage message)
        {
            var packet = MessageParser.ToPacket(message);
            await client.SendPacket(packet);
        }
    }
}
