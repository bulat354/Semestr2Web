using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Test
{
    public class StringMessage
    {
        public string Message { get; set; }

        public StringMessage() { }

        public StringMessage(string message) 
        { 
            Message = message;
        }

        public DPTPPacket ToPacket()
        {
            var packet = DPTPPacket.Create(0, 0);
            if (Message != null)
                packet.SetValueRaw(0, Encoding.ASCII.GetBytes(Message));

            return packet;
        }

        public static StringMessage Parse(DPTPPacket packet)
        {
            var message = new StringMessage();
            if (packet.GetField(0) != null)
                message.Message = Encoding.ASCII.GetString(packet.GetValueRaw(0));

            return message;
        }

        public static implicit operator StringMessage(string str) => new StringMessage(str);

        public static implicit operator string(StringMessage message) => message.Message;

        public string[] Split()
        {
            return Message.Split(' ');
        }
    }
}
