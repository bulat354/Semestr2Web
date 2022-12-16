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
            throw new NotImplementedException();
        }

        public byte GetPacketSubtype()
        {
            throw new NotImplementedException();
        }

        public byte GetPacketType()
        {
            throw new NotImplementedException();
        }

        public void SetPacketFields(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }

        public void SetPacketSubtype(byte subtype)
        {
            throw new NotImplementedException();
        }
    }
}
