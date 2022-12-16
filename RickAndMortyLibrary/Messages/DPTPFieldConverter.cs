using MyProtocol;
using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public class DPTPFieldConverter
    {
        public static DPTPPacketField? ToField(byte id, int? obj)
        {
            if (obj == null)
                return null;

            var bytes = BitConverter.GetBytes(obj.Value);
            return ToField(id, bytes);
        }

        public static DPTPPacketField? ToField(byte id, string? obj)
        {
            if (obj == null)
                return null;

            var bytes = Encoding.UTF8.GetBytes(obj);
            return ToField(id, bytes);
        }

        public static DPTPPacketField? ToField(byte id, bool? obj)
        {
            if (obj == null)
                return null;

            var bytes = new[] { (byte)(obj.Value ? 0 : 1) };
            return ToField(id, bytes);
        }

        public static DPTPPacketField? ToField(byte id, byte? obj)
        {
            if (obj == null)
                return null;

            var bytes = new[] { obj.Value };
            return ToField(id, bytes);
        }

        public static DPTPPacketField? ToField(byte id, byte[]? bytes)
        {
            if (bytes == null)
                return null;

            return new DPTPPacketField()
            {
                FieldID = id,
                FieldSize = (byte)bytes.Length,
                Contents = bytes
            };
        }


        public static int? ToInt(DPTPPacket packet, byte id)
        {
            var field = packet.GetField(id);
            if (field != null)
                return BitConverter.ToInt32(field.Contents, 0);

            return null;
        }

        public static string? ToString(DPTPPacket packet, byte id)
        {
            var field = packet.GetField(id);
            if (field != null)
                return Encoding.UTF8.GetString(field.Contents);

            return null;
        }

        public static bool? ToBool(DPTPPacket packet, byte id)
        {
            var field = packet.GetField(id);
            if (field != null)
                return field.Contents[0] > 0 ? true : false;

            return null;
        }

        public static byte? ToByte(DPTPPacket packet, byte id)
        {
            var field = packet.GetField(id);
            if (field != null)
                return field.Contents[0];

            return null;
        }

        public static byte[]? ToBytes(DPTPPacket packet, byte id)
        {
            var field = packet.GetField(id);
            if (field != null)
                return field.Contents;

            return null;
        }

        public static ActionCard? ToActionCard(DPTPPacket packet, byte id)
        {
            var cardId = ToInt(packet, id);
            if (cardId != null)
                return CardsImporter.GetCard<ActionCard>(cardId.Value);

            return null;
        }

        public static CharacterCard? ToCharacterCard(DPTPPacket packet, byte id)
        {
            var cardId = ToInt(packet, id);
            if (cardId != null)
                return CardsImporter.GetCard<CharacterCard>(cardId.Value);

            return null;
        }

        public static PersonalityCard? ToPersonalityCard(DPTPPacket packet, byte id)
        {
            var cardId = ToInt(packet, id);
            if (cardId != null)
                return CardsImporter.GetCard<PersonalityCard>(cardId.Value);

            return null;
        }
    }
}
