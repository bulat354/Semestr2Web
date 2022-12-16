using MyProtocol;
using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public class MessageParser
    {
        public static Dictionary<byte, Type> dict;

        static MessageParser()
        {
            dict = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(PacketTypeAttribute)) && x.IsSubclassOf(typeof(IMessage)))
                .ToDictionary(x => x.GetCustomAttribute<PacketTypeAttribute>().PacketType);
        }

        public static IMessage Parse(DPTPPacket packet)
        {
            var message = (IMessage)Activator.CreateInstance(dict[packet.PacketType]);

            message.SetPacketFields(packet);
            message.SetPacketSubtype(packet.PacketSubtype);

            return message;
        }

        public static DPTPPacket ToPacket(IMessage message)
        {
            var type = message.GetPacketType();
            var subtype = message.GetPacketSubtype();

            var packet = DPTPPacket.Create(type, subtype);

            foreach (var field in message.GetPacketFields())
            {
                if (field != null)
                    packet.Fields.Add(field);
            }
            
            return packet;
        }

        #region Player Message
        public static PlayerMessage JoinPlayer(string name)
            => new PlayerMessage()
            {
                Goal = PlayerMessageGoal.Join,
                UserName = name
            };

        public static PlayerMessage DisconnectPlayer(string name)
            => new PlayerMessage()
            {
                Goal = PlayerMessageGoal.Disconnect,
                UserName = name
            };

        public static PlayerMessage FailPlayer(string name)
            => new PlayerMessage()
            {
                Goal = PlayerMessageGoal.Fail,
                UserName = name
            };

        public static PlayerMessage SelectPlayer(string name)
            => new PlayerMessage()
            {
                Goal = PlayerMessageGoal.Select,
                UserName = name
            };

        public static PlayerMessage WaitForSelectPlayer()
            => new PlayerMessage()
            {
                Goal = PlayerMessageGoal.WaitSelect
            };
        #endregion

        #region Card Message
        public static CardMessage AddCardToHand(ActionCard actionCard)
            => new CardMessage()
            {
                Card = actionCard,
                Goal = CardMessageGoal.AddToHand,
            };

        public static CardMessage GetCardFromHand()
            => new CardMessage()
            {
                Goal = CardMessageGoal.RequestChoosing
            };

        public static CardMessage SendCardFromHand(ActionCard card)
            => new CardMessage()
            {
                Card = card,
                Goal = CardMessageGoal.GetFromHand
            };

        public static CardMessage ShowPackTop(PersonalityCard card)
            => new CardMessage()
            {
                Card = card,
                Goal = CardMessageGoal.ShowPack,
            };
        #endregion

        #region Character Message
        public static CharacterMessage AddToTable(Character character)
            => new CharacterMessage()
            {
                Character = character,
                Goal = CharacterMessageGoal.AddToTable,
            };

        public static CharacterMessage RemoveFromTable(Character character, int timeOut = 0)
            => new CharacterMessage()
            {
                Goal = CharacterMessageGoal.RemoveFromTable,
                TimeOut = timeOut
            };

        public static CharacterMessage AttachToPlayer(Character? character, string userName)
            => new CharacterMessage()
            {
                Character = character,
                UserName = userName,
                Goal = CharacterMessageGoal.AddToPlayer
            };

        public static CharacterMessage SelectCharacter(Character? character)
            => new CharacterMessage()
            {
                Character = character,
                Goal = CharacterMessageGoal.Select
            };

        public static CharacterMessage WaitForSelectCharacter()
            => new CharacterMessage()
            {
                Goal = CharacterMessageGoal.WaitSelect
            };

        public static CharacterMessage RevealCharacter(Character character)
            => new CharacterMessage()
            {
                Character = character,
                Goal = CharacterMessageGoal.Reveal
            };
        #endregion

        #region Vote Messages
        public static VoteMessage StartVoting()
            => new VoteMessage()
            {
                Goal = VoteMessageGoal.Start
            };

        public static VoteMessage ToVoteScreen()
            => new VoteMessage()
            {
                Goal = VoteMessageGoal.ToVoteState
            };

        public static VoteMessage FromVoteScreen()
            => new VoteMessage()
            {
                Goal = VoteMessageGoal.FromVoteState
            };

        public static VoteMessage RequestVoteResult()
            => new VoteMessage()
            {
                Goal = VoteMessageGoal.WaitForResult
            };

        public static VoteMessage SendVoteResult(bool result)
            => new VoteMessage()
            {
                Goal = VoteMessageGoal.Result,
                Result = result
            };
        #endregion

        #region Timer Message
        public static TimerMessage StartTimer(int sec)
            => new TimerMessage()
            {
                Seconds = sec,
                Goal = TimerMessageGoal.Start
            };

        public static TimerMessage StopTimer()
            => new TimerMessage()
            {
                Goal = TimerMessageGoal.Stop
            };
        #endregion

        #region Game Over Message
        public static GameOverMessage Win(bool isTrue)
            => new GameOverMessage()
            {
                IsWinner = isTrue
            };
        #endregion

        #region Colors Message
        public static ColorsMessage WaitSelectColor(CardColor[] colors)
            => new ColorsMessage()
            {
                Colors = colors,
                Goal = ColorsMessageGoal.WaitSelect
            };

        public static ColorsMessage SelectColor(CardColor color)
            => new ColorsMessage()
            {
                Color = color,
                Goal = ColorsMessageGoal.Select
            };
        #endregion

        #region
        #endregion
    }
}
