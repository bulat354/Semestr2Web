using MyProtocol;
using MyProtocol.Attributes;
using RickAndMortyLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ErrorMessages
{
    public class Error : IMessage
    {
        protected byte[] messageBytes;
        public string MessageString { get; }

        private Error(string message)
        {
            MessageString = message;
            messageBytes = Encoding.UTF8.GetBytes(message);
        }

        public static Error MaxPlayers()
        {
            return new Error("Максимальное число игроков");
        }

        public static Error MinPlayers()
        {
            return new Error("Недостаточное число игроков");
        }

        public DPTPPacket ToPacket()
        {
            throw new NotImplementedException();
        }

        public void Parse(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
