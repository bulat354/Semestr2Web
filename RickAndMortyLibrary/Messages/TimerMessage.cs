using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [PacketType(6)]
    public class TimerMessage : IMessage
    {
        //subtype
        public TimerMessageGoal Goal { get; set; }

        public int? Seconds { get; set; }

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

    public enum TimerMessageGoal
    {
        Start, Stop
    }
}
