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
            if (Card != null)
                yield return DPTPFieldConverter.ToField(0, Card.Id);
        }

        public byte GetPacketSubtype()
        {
            return (byte)Goal;
        }

        public byte GetPacketType()
        {
            return 1;
        }

        public void SetPacketFields(DPTPPacket packet)
        {
            switch (Goal)
            {
                case CardMessageGoal.AddToHand:
                case CardMessageGoal.GetFromHand:
                    Card = DPTPFieldConverter.ToActionCard(packet, 0);
                    break;

                case CardMessageGoal.ShowPack:
                    Card = DPTPFieldConverter.ToPersonalityCard(packet, 0);
                    break;
            }
        }

        public void SetPacketSubtype(byte subtype)
        {
            Goal = (CardMessageGoal)subtype;
        }
    }

    public enum CardMessageGoal
    {
        AddToHand, RequestChoosing, GetFromHand, ShowPack
    }
}
