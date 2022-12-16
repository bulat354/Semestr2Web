using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public class PlayerMessage : IMessage
    {
        public string UserName { get; set; }
        // для Subtype
        public PlayerMessageGoal Goal { get; set; }

        public void Parse(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }

        public DPTPPacket ToPacket()
        {
            throw new NotImplementedException();
        }
    }

    public enum PlayerMessageGoal
    {
        Join, Disconnect, Fail, Select, WaitSelect
    }
}
