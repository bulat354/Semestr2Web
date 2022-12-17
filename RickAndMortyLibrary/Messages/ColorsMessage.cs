using MyProtocol;
using RickAndMortyLibrary.Common.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [PacketType(2)]
    public class ColorsMessage : IMessage
    {
        //subtype
        public ColorsMessageGoal Goal { get; set; }

        public CardColor[]? Colors { get; set; }
        public CardColor? Color { get; set; }

        public IEnumerable<DPTPPacketField?> GetPacketFields()
        {
            if (Color != null)
                yield return DPTPFieldConverter.ToField(0, (byte)Color);
            
            if (Colors != null)
                yield return DPTPFieldConverter.ToField(0, Colors.Select(c => (byte)c).ToArray());
        }

        public byte GetPacketSubtype()
        {
            return (byte)Goal;
        }

        public byte GetPacketType()
        {
            return 2;
        }

        public void SetPacketFields(DPTPPacket packet)
        {
            var color = DPTPFieldConverter.ToInt(packet, 0);
            Color = (color != null) ? (CardColor)color : null;

            Colors = DPTPFieldConverter.ToBytes(packet, 1)?.Select(p => (CardColor)p).ToArray();
        }

        public void SetPacketSubtype(byte subtype)
        {
            Goal = (ColorsMessageGoal)subtype;
        }
    }

    public enum ColorsMessageGoal
    {
        WaitSelect, Select
    }
}
