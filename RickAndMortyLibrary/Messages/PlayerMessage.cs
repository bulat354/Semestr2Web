using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [PacketType(5)]
    public class PlayerMessage : IMessage
    {
        //subtype
        public PlayerMessageGoal Goal { get; set; }

        public string? UserName { get; set; }

        public IEnumerable<DPTPPacketField?> GetPacketFields()
        {
            yield return DPTPFieldConverter.ToField(0, UserName);
        }

        public byte GetPacketSubtype()
        {
            return (byte)Goal;
        }

        public byte GetPacketType()
        {
            return 5;
        }

        public void SetPacketFields(DPTPPacket packet)
        {
            UserName = DPTPFieldConverter.ToString(packet, 0);
        }

        public void SetPacketSubtype(byte subtype)
        {
            Goal = (PlayerMessageGoal)subtype;
        }
    }

    public enum PlayerMessageGoal
    {
        Join, Disconnect, Fail, Select, WaitSelect
    }
}
