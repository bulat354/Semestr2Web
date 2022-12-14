using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ErrorMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ServerSide
{
    /// <summary>
    /// Игрок-хост
    /// </summary>
    internal class HostPlayer : IPlayer
    {
        public string UserName { get; set; }

        private IPlayerUI ui;

        public HostPlayer(string userName, IPlayerUI ui)
        {
            UserName = userName;
            this.ui = ui;
        }

        public async Task GetReady()
        {
            await ui.WaitForPressStart();
        }

        public void SendError(Error error)
        {
            ui.ShowError(error.MessageString);
        }

        public void SendNewPlayer(IPlayer player)
        {
            ui.AddPlayer(player.UserName);
        }

        public void SendRemovePlayer(IPlayer player)
        {
            ui.RemovePlayer(player.UserName);
        }

        public void AddCard(ActionCard actionCard)
        {
            ui.AddCardToHand(actionCard);
        }

        public void AddCharacter(Character character)
        {
            ui.AddCharacter(character);
        }

        public async Task WaitForVote(CancellationToken stopWaiting)
        {
            await ui.WaitForStartVoting().WaitAsync(stopWaiting);
        }

        public void StartVoting()
        {
            ui.StartVoting();
        }

        public async Task<bool> WaitVoteResult()
        {
            return await ui.WaitForVotingResult();
        }

        public void StopVoting()
        {
            ui.StopVoting();
        }

        public void StartTimer(int seconds)
        {
            ui.StartTimer(seconds);
        }

        public void StopTimer()
        {
            ui.StopTimer();
        }

        public async Task<ActionCard?> WaitChoosingAction(CancellationToken stopWaiting)
        {
            return await ui.ChooseActionFromHand();
        }

        public void Win()
        {
            ui.Win();
        }

        public void Lose()
        {
            ui.Lose();
        }

        public Character GetCharacter()
        {
            var task = ui.GetCharacter();
            task.Wait();
            return task.Result;
        }

        public void AttachCharacter(Character character, string userName)
        {
            ui.SetCharacter(character, userName);
        }

        public Person GetPerson()
        {
            return GetCharacter().Personality.Person;
        }
    }
}
