using MyProtocol;
using MyProtocol.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [PacketType(3)]
    public class Error : IMessage
    {
        public string MessageString { get; }

        private Error(string message)
        {
            MessageString = message;
        }

        public static Error MaxPlayers()
        {
            return new Error("Максимальное число игроков");
        }

        public static Error MinPlayers()
        {
            return new Error("Недостаточное число игроков");
        }

        public static Error WrongCharacter()
        {
            return new Error("Выбери другого персонажа");
        }

        public byte GetPacketType()
        {
            throw new NotImplementedException();
        }

        public byte GetPacketSubtype()
        {
            throw new NotImplementedException();
        }

        public void SetPacketSubtype(byte subtype)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DPTPPacketField?> GetPacketFields()
        {
            throw new NotImplementedException();
        }

        public void SetPacketFields(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
