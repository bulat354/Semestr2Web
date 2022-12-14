using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public class TimerMessage : IMessage
    {
        public int Seconds { get; set; }
        // для Subtype
        public TimerMessageGoal Goal { get; set; }

        public void Parse(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }

        public DPTPPacket ToPacket()
        {
            throw new NotImplementedException();
        }
    }

    public enum TimerMessageGoal
    {
        Start, Stop
    }
}
