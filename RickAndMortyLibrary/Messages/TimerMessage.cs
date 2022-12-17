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
            yield return DPTPFieldConverter.ToField(0, Seconds);
        }

        public byte GetPacketSubtype()
        {
            return (byte)Goal;
        }

        public byte GetPacketType()
        {
            return 6;
        }

        public void SetPacketFields(DPTPPacket packet)
        {
            Seconds = DPTPFieldConverter.ToInt(packet, 0);
        }

        public void SetPacketSubtype(byte subtype)
        {
            Goal = (TimerMessageGoal)subtype;
        }
    }

    public enum TimerMessageGoal
    {
        Start, Stop
    }
}
