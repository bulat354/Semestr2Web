using MyProtocol.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ErrorMessages
{
    [DPTPType(0, 0)]
    public class Error
    {
        [DPTPField(0)]
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
    }
}
