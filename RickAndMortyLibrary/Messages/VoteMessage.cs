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

        public bool Result { get; set; }

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

    public enum VoteMessageGoal
    {
        Start, ToVoteState, FromVoteState, WaitForResult, Result
    }
}
