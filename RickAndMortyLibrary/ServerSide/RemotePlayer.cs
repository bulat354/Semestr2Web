using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ErrorMessages;
using RickAndMortyLibrary.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ServerSide
{
    internal class RemotePlayer : IPlayer
    {
        public string UserName { get; set; }
        public TaskCompletionSource Disconnected { get; }

        private IClient client;

        private LinkedList<Tuple<Func<IMessage, bool>, TaskCompletionSource<IMessage>>> waitHandles { get; set; }

        private Character? character;

        public RemotePlayer(IClient client)
        {
            this.client = client;

            Disconnected = new TaskCompletionSource();

            waitHandles = new LinkedList<Tuple<Func<IMessage, bool>, TaskCompletionSource<IMessage>>>();
        }

        public void Disconnect()
        {
            client.Disconnect();
            Disconnected.SetResult();
        }

        public async void StartReceiving()
        {
            UserName = ((PlayerMessage)await client.Receive()).UserName;

            while (true)
            {
                var message = await client.Receive();
                
                lock (waitHandles)
                {
                    var last = waitHandles.First;
                    while (last != null)
                    {
                        var tuple = last.Value;
                        var predicate = tuple.Item1;
                        var task = tuple.Item2;

                        if (task.Task.IsCanceled)
                        {
                            waitHandles.Remove(last);
                        }
                        else if (predicate(message))
                        {
                            task.SetResult(message);
                            waitHandles.Remove(last);
                            break;
                        }

                        last = last.Next;
                    }
                }
            }
        }

        public async Task<T> WaitFor<T>(Func<T, bool> predicate) where T : IMessage
        {
            var source = new TaskCompletionSource<IMessage>();

            var func = new Func<IMessage, bool>(m => m is T && predicate((T)m));
            var tuple = Tuple.Create(func, source);

            var result = await source.Task;
            return (T)result;
        }

        public async Task<T> WaitFor<T>(Func<T, bool> predicate, CancellationToken token) where T : IMessage
        {
            var source = new TaskCompletionSource<IMessage>();
            source.SetCanceled(token);

            var func = new Func<IMessage, bool>(m => m is T && predicate((T)m));
            var tuple = Tuple.Create(func, source);

            var result = await source.Task;
            return (T)result;
        }

        public void SendError(Error error)
        {
            client.Send(error);
        }

        public void SendNewPlayer(IPlayer player)
        {
            client.Send(MessageParser.JoinPlayer(player.UserName));
        }

        public void SendRemovePlayer(IPlayer player)
        {
            client.Send(MessageParser.DisconnectPlayer(player.UserName));
        }

        public void AddCard(ActionCard actionCard)
        {
            client.Send(MessageParser.AddCardToHand(actionCard));
        }

        public void AddCharacter(Character character)
        {
            client.Send(MessageParser.AddToTable(character));
        }

        public async Task WaitForVote(CancellationToken stopWaiting)
        {
            await WaitFor<VoteMessage>(x => x.Goal == VoteMessageGoal.Start, stopWaiting);
        }

        public void StartVoting()
        {
            client.Send(MessageParser.ToVoteScreen());
        }

        public async Task<bool> WaitVoteResult()
        {
            client.Send(MessageParser.RequestVoteResult());

            var message = await WaitFor<VoteMessage>(x => x.Goal == VoteMessageGoal.Result);
            return message.Result;
        }

        public void StopVoting()
        {
            client.Send(MessageParser.FromVoteScreen());
        }

        public void StartTimer(int seconds)
        {
            client.Send(MessageParser.StartTimer(seconds));
        }

        public void StopTimer()
        {
            client.Send(MessageParser.StopTimer());
        }

        public async Task<ActionCard?> WaitChoosingAction(CancellationToken stopWaiting)
        {
            client.Send(MessageParser.GetCardFromHand());

            var message = await WaitFor<CardMessage<ActionCard>>(x => x.Goal == CardMessageGoal.GetFromHand, stopWaiting);
            return message?.Card;
        }

        public void Win()
        {
            client.Send(MessageParser.Win(true));
        }

        public void Lose()
        {
            client.Send(MessageParser.Win(false));
        }

        public Character? GetCharacter()
        {
            AttachCharacter(character);
            return character;
        }

        public void AttachCharacter(Character? character, string? userName = null)
        {
            userName ??= UserName;
            this.character = character;

            client.Send(MessageParser.AttachToPlayer(character, userName));
        }

        public Person GetPerson()
        {
            AttachCharacter(character);

            if (character == null)
                return Person.Friend;

            return character.Personality.Person;
        }
    }
}
