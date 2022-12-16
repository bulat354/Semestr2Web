using MyProtocol;
using RickAndMortyLibrary.Common.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public class CardMessage<T> : IMessage
        where T : Card
    {
        public bool IsRequest { get; set; }

        public T? Card
        {
            get { return CardsImporter.GetCard<T>(cardId); }
            set { cardId = value.Id; }
        }
        // для Subtype
        public CardMessageGoal Goal { get; set; }

        private int cardId;

        public void Parse(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }

        public DPTPPacket ToPacket()
        {
            throw new NotImplementedException();
        }
    }

    public enum CardMessageGoal
    {
        AddToHand, RequestChoosing, GetFromHand, ShowPack
    }
}
