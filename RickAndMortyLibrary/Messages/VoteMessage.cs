using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [PacketType(7)]
    public class VoteMessage : IMessage
    {
        //subtype
        public VoteMessageGoal Goal { get; set; }

        public bool? Result { get; set; }

        public IEnumerable<DPTPPacketField?> GetPacketFields()
        {
            yield return DPTPFieldConverter.ToField(0, Result);
        }

        public byte GetPacketSubtype()
        {
            return (byte)Goal;
        }

        public byte GetPacketType()
        {
            return 7;
        }

        public void SetPacketFields(DPTPPacket packet)
        {
            Result = DPTPFieldConverter.ToBool(packet, 0);
        }

        public void SetPacketSubtype(byte subtype)
        {
            Goal = (VoteMessageGoal)subtype;
        }
    }

    public enum VoteMessageGoal
    {
        Start, ToVoteState, FromVoteState, WaitForResult, Result
    }
}
