using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public class VoteMessage : IMessage
    {
        public bool IsRequest { get; set; }

        public bool Result { get; set; }
        // для Subtype
        public VoteMessageGoal Goal { get; set; }

        public void Parse(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }

        public DPTPPacket ToPacket()
        {
            throw new NotImplementedException();
        }
    }

    public enum VoteMessageGoal
    {
        Start, ToVoteState, FromVoteState, WaitForResult, Result
    }
}
