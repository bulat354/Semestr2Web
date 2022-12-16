using MyProtocol;
using RickAndMortyLibrary.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [PacketType(0)]
    public class CharacterMessage : IMessage
    {
        //subtype
        public CharacterMessageGoal Goal { get; set; }

        public Character? Character { get; set; }
        public int? TimeOut { get; set; }
        public string? UserName { get; set; }

        public byte GetPacketType()
        {
            return 0;
        }

        public byte GetPacketSubtype()
        {
            return (byte)Goal;
        }

        public void SetPacketSubtype(byte subtype)
        {
            Goal = (CharacterMessageGoal)subtype;
        }

        public IEnumerable<DPTPPacketField?> GetPacketFields()
        {
            if (Character != null)
            {
                yield return DPTPFieldConverter.ToField(0, Character.Card.Id);
                yield return DPTPFieldConverter.ToField(1, Character.Personality.Id);
                yield return DPTPFieldConverter.ToField(2, Character.IsKillable);
                yield return DPTPFieldConverter.ToField(3, Character.IsAttachedToPlayer);
                yield return DPTPFieldConverter.ToField(4, Character.Immutable);
            }

            yield return DPTPFieldConverter.ToField(5, TimeOut);
            yield return DPTPFieldConverter.ToField(6, UserName);
        }

        public void SetPacketFields(DPTPPacket packet)
        {
            var card = DPTPFieldConverter.ToCharacterCard(packet, 0);

            if (card != null)
            {
                Character = new Character();
                Character.Card = card;
                Character.Personality = DPTPFieldConverter.ToPersonalityCard(packet, 1);
                Character.IsKillable = DPTPFieldConverter.ToBool(packet, 2).Value;
                Character.IsAttachedToPlayer = DPTPFieldConverter.ToBool(packet, 3).Value;
                Character.Immutable = DPTPFieldConverter.ToBool(packet, 4).Value;
            }

            TimeOut = DPTPFieldConverter.ToInt(packet, 0);
            UserName = DPTPFieldConverter.ToString(packet, 0);
        }
    }

    public enum CharacterMessageGoal
    {
        AddToTable, RemoveFromTable, AddToPlayer, GetFromPlayer, WaitSelect, Select, Reveal
    }
}
