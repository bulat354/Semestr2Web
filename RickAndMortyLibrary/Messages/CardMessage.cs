﻿using MyProtocol;
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

        public T? Card { get; set; }
        // для Subtype
        public CardMessageGoal Goal { get; set; }

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
        AddToHand, RequestChoosing, GetFromHand
    }
}