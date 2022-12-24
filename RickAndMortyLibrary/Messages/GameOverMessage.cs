using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [PacketType(4)]
    public class GameOverMessage : IMessage
    {
        //subtype
        public bool IsWinner { get; set; }

        public IEnumerable<DPTPPacketField?> GetPacketFields()
        {
            yield break;
        }

        public byte GetPacketSubtype()
        {
            return Convert.ToByte(IsWinner);
        }

        public byte GetPacketType()
        {
            return 4;
        }

        public void SetPacketFields(DPTPPacket packet) { }

        public void SetPacketSubtype(byte subtype)
        {
            IsWinner = Convert.ToBoolean(subtype);
        }
    }
}
