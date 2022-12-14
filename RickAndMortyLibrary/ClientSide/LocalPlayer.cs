using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ClientSide
{
    public class LocalPlayer
    {
        public string UserName { get; set; }

        private IPlayerUI ui;
        private IClient client;

        public async Task Start(IMainUI ui)
        {
            client = new Client();

            await ui.WaitForConnecting(out var ip, out var port, out var userName);

            UserName = userName;
            client.Connect(ip, port);
            client.Send(MessageParser.JoinPlayer(userName));

            this.ui = await ui.ToLocalPlayerScreen();

            ReceiveAndSendMessages();
        }

        private async void ReceiveAndSendMessages()
        {
            while (true)
            {
                var message = await client.Receive();
                var responce = await ProcessMessage(message);

                if (responce != null)
                    client.Send(responce);
            }
        }

        private async Task<IMessage> ProcessMessage(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            client.Disconnect();
        }
    }
}
