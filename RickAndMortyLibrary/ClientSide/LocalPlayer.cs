using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ClientSide
{
    public class LocalPlayer
    {
        public string UserName { get; set; }

        private IPlayerUI ui;
        private IClient client;

        public async Task Start(IMainUI ui)
        {
            client = new Client();

            await ui.WaitForConnecting(out var ip, out var port, out var userName);

            UserName = userName;
            client.Connect(ip, port);
            client.Send(MessageParser.JoinPlayer(userName));

            this.ui = await ui.ToLocalPlayerScreen();

            ReceiveAndSendMessages();
            WaitForVotePress();
        }

        private async void ReceiveAndSendMessages()
        {
            while (true)
            {
                var message = await client.Receive();
                var responce = await ProcessMessage(message);

                if (responce != null)
                    client.Send(responce);
            }
        }

        private async void WaitForVotePress()
        {
            while (true)
            {
                await ui.WaitForStartVoting();

                client.Send(MessageParser.StartVoting());
            }
        }

        private async Task<IMessage?> ProcessMessage(IMessage message)
        {
            if (message is CardMessage<ActionCard> obj1)
                return await Process(obj1);
            else if (message is CardMessage<CharacterCard> obj2)
                return await Process(obj2);
            else if (message is CardMessage<PersonalityCard> obj3)
                return await Process(obj3);
            else if (message is CharacterMessage obj4)
                return await Process(obj4);
            else if (message is ColorsMessage obj5)
                return await Process(obj5);
            else if (message is Error obj6)
                return await Process(obj6);
            else if (message is GameOverMessage obj7)
                return await Process(obj7);
            else if (message is PlayerMessage obj8)
                return await Process(obj8);
            else if (message is TimerMessage obj9)
                return await Process(obj9);
            else if (message is VoteMessage obj10)
                return await Process(obj10);
            else
                return null;
        }

        private async Task<IMessage?> Process(CardMessage<ActionCard> message)
        {
            switch (message.Goal)
            {
                case CardMessageGoal.AddToHand:
                    ui.AddCardToHand(message.Card);
                    break;
                case CardMessageGoal.RequestChoosing:
                    return MessageParser.SendCardFromHand(await ui.ChooseActionFromHand());
            }

            return null;
        }

        private async Task<IMessage?> Process(CardMessage<CharacterCard> message)
        {
            throw new NotImplementedException();
        }

        private async Task<IMessage?> Process(CardMessage<PersonalityCard> message)
        {
            switch (message.Goal)
            {
                case CardMessageGoal.ShowPack:
                    ui.ShowTopFromPack(message.Card);
                    break;
            }

            return null;
        }

        private async Task<IMessage?> Process(CharacterMessage message)
        {
            switch (message.Goal)
            {
                case CharacterMessageGoal.AddToTable:
                    ui.AddCharacter(message.Character);
                    break;
                case CharacterMessageGoal.RemoveFromTable:
                    ui.RemoveCharacter(message.Character, message.TimeOut);
                    break;
                case CharacterMessageGoal.AddToPlayer:
                    ui.SetCharacter(message.Character, message.UserName);
                    break;
                case CharacterMessageGoal.WaitSelect:
                    return MessageParser.SelectCharacter(await ui.SelectCharacter());
                case CharacterMessageGoal.Reveal:
                    ui.ShowCharacterPerson(message.Character);
                    break;
            }

            return null;
        }

        private async Task<IMessage?> Process(ColorsMessage message)
        {
            switch (message.Goal)
            {
                case ColorsMessageGoal.WaitSelect:
                    return MessageParser.SelectColor(await ui.SelectColor(message.Colors));
            }

            return null;
        }

        private async Task<IMessage?> Process(Error message)
        {
            ui.ShowError(message.MessageString);

            return null;
        }

        private async Task<IMessage?> Process(GameOverMessage message)
        {
            if (message.IsWinner)
                ui.Win();
            else
                ui.Lose();

            return null;
        }

        private async Task<IMessage?> Process(PlayerMessage message)
        {
            switch (message.Goal)
            {
                case PlayerMessageGoal.Join:
                    ui.AddPlayer(message.UserName);
                    break;
                case PlayerMessageGoal.Disconnect:
                    ui.RemovePlayer(message.UserName);
                    break;
                case PlayerMessageGoal.Fail:
                    ui.PlayerFailed(message.UserName);
                    break;
                case PlayerMessageGoal.WaitSelect:
                    return MessageParser.SelectPlayer(await ui.SelectPlayer());
            }

            return null;
        }

        private async Task<IMessage?> Process(TimerMessage message)
        {
            switch (message.Goal)
            {
                case TimerMessageGoal.Start:
                    ui.StartTimer(message.Seconds);
                    break;
                case TimerMessageGoal.Stop:
                    ui.StopTimer();
                    break;
            }

            return null;
        }

        private async Task<IMessage?> Process(VoteMessage message)
        {
            switch (message.Goal)
            {
                case VoteMessageGoal.ToVoteState:
                    ui.StartVoting();
                    break;
                case VoteMessageGoal.FromVoteState:
                    ui.StopVoting();
                    break;
                case VoteMessageGoal.WaitForResult:
                    return MessageParser.SendVoteResult(await ui.WaitForVotingResult());
            }

            return null;
        }

        public void Stop()
        {
            client.Disconnect();
        }
    }
}
