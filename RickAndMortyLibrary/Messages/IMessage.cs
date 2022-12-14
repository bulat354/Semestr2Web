using MyProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public interface IMessage
    {
        DPTPPacket ToPacket();
        void Parse(DPTPPacket packet);
    }
}
