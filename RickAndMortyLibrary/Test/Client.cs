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

        private List<StringMessage> _readyMessages = new List<StringMessage>();

        public StringMessage? WaitForMessage(MessageFirstGoal firstGoal, MessageSecondGoal secondGoal)
        {
            lock (_readyMessages)
            {
                foreach (var message in _readyMessages)
                {
                    if (firstGoal.HasFlag(message.FirstGoal) && secondGoal.HasFlag(message.SecondGoal))
                    {
                        _readyMessages.Remove(message);
                        return message;
                    }
                }
            }

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
                            if (firstGoal.HasFlag(msg.FirstGoal) && secondGoal.HasFlag(msg.SecondGoal))
                                return msg;
                            else
                                lock (_readyMessages)
                                    _readyMessages.Add(msg);
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public StringMessage? WaitForAny()
        {
            lock (_readyMessages)
            {
                if (_readyMessages.Count > 0)
                {
                    var message = _readyMessages[0];
                    _readyMessages.RemoveAt(0);
                    return message;
                }
            }

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
