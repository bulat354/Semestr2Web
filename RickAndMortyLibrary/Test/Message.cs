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
        public MessageFirstGoal FirstGoal { get; set; }
        public MessageSecondGoal SecondGoal { get; set; }
        public string? Message { get; set; }

        public StringMessage() { }
        public StringMessage(MessageFirstGoal firstGoal, string? message = null, MessageSecondGoal secondGoal = MessageSecondGoal.None)
        {
            FirstGoal = firstGoal;
            SecondGoal = secondGoal;
            Message = message;
        }

        public DPTPPacket ToPacket()
        {
            var packet = DPTPPacket.Create((byte)FirstGoal, (byte)SecondGoal);
            if (Message != null)
                packet.SetValueRaw(0, Encoding.UTF8.GetBytes(Message));

            return packet;
        }

        public static StringMessage Parse(DPTPPacket packet)
        {
            var message = new StringMessage();
            if (packet.GetField(0) != null)
                message.Message = Encoding.UTF8.GetString(packet.GetValueRaw(0));
            message.FirstGoal = (MessageFirstGoal)packet.PacketType;
            message.SecondGoal = (MessageSecondGoal)packet.PacketSubtype;

            return message;
        }

        public int ToInt() => int.Parse(Message);

        public static StringMessage Create(MessageFirstGoal firstGoal, string? message = null, MessageSecondGoal secondGoal = MessageSecondGoal.None)
        {
            return new StringMessage(firstGoal, message, secondGoal);
        }

        public static StringMessage CreateMessage(string message)
        {
            return new StringMessage(MessageFirstGoal.Message, message);
        }
    }
}
