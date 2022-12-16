using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public interface IMessage
    {
        byte GetPacketType();

        byte GetPacketSubtype();
        void SetPacketSubtype(byte subtype);

        IEnumerable<DPTPPacketField?> GetPacketFields();
        void SetPacketFields(DPTPPacket packet);
    }
}
