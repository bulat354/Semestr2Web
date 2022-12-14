using MyProtocol;
using RickAndMortyLibrary.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public class CharacterMessage : IMessage
    {
        public bool IsRequest { get; set; }

        public Character? Character { get; set; }
        // для Subtype
        public CharacterMessageGoal Goal { get; set; }

        public string UserName { get; set; }

        public void Parse(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }

        public DPTPPacket ToPacket()
        {
            throw new NotImplementedException();
        }
    }

    public enum CharacterMessageGoal
    {
        AddToTable, RemoveFromTable, AddToPlayer, GetFromPlayer
    }
}
