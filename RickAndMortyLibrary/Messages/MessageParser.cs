using MyProtocol;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    public class MessageParser
    {
        public static IMessage Parse(DPTPPacket packet)
        {
            throw new NotImplementedException();
        }

        #region Player Message
        public static PlayerMessage JoinPlayer(string name)
            => new PlayerMessage()
            {
                DoesJoin = true,
                UserName = name
            };

        public static PlayerMessage DisconnectPlayer(string name)
            => new PlayerMessage()
            {
                DoesJoin = false,
                UserName = name
            };
        #endregion

        #region Card Message
        public static CardMessage<ActionCard> AddCardToHand(ActionCard actionCard)
            => new CardMessage<ActionCard>()
            {
                Card = actionCard,
                Goal = CardMessageGoal.AddToHand,
            };

        public static CardMessage<ActionCard> GetCardFromHand()
            => new CardMessage<ActionCard>()
            {
                IsRequest = true,
                Goal = CardMessageGoal.RequestChoosing
            };
        #endregion

        #region
        public static CharacterMessage AddToTable(Character character)
            => new CharacterMessage()
            {
                Character = character,
                Goal = CharacterMessageGoal.AddToTable,
            };

        public static CharacterMessage RemoveFromTable(Character character)
            => new CharacterMessage()
            {
                Goal = CharacterMessageGoal.RemoveFromTable
            };

        public static CharacterMessage AttachToPlayer(Character? character, string userName)
            => new CharacterMessage()
            {
                Character = character,
                UserName = userName,
                Goal = CharacterMessageGoal.AddToPlayer
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
                Goal = VoteMessageGoal.WaitForResult,
                IsRequest = true
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

        #region
        #endregion
    }
}
