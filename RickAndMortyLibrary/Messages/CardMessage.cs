using MyProtocol;
using RickAndMortyLibrary.Common.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [PacketType(1)]
    public class CardMessage : IMessage
    {
        //subtype
        public CardMessageGoal Goal { get; set; }

        public Card? Card { get; set; }

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

    public enum CardMessageGoal
    {
        AddToHand, RequestChoosing, GetFromHand, ShowPack
    }
}
