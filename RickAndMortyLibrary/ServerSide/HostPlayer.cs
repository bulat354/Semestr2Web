using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.Messages;
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
        public int Number { get; set; }

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

        #region Card
        public void TakeCard(ActionCard actionCard)
        {
            ui.AddCardToHand(actionCard);
        }

        private ActionCard? next = null;

        public async Task<ActionCard?> WaitChoosingAction(CancellationToken stopWaiting)
        {
            if (next == null)
                return await ui.ChooseActionFromHand();

            var tmp = next;
            next = null;
            return tmp;
        }

        public void ShowTopFromPack(PersonalityCard card)
        {
            ui.ShowTopFromPack(card);
        }
        #endregion

        #region Character
        public void AddCharacter(Character character)
        {
            ui.AddCharacter(character);
        }

        public void RemoveCharacter(Character character, int timeOut = 0)
        {
            ui.RemoveCharacter(character, timeOut);
        }

        public void AttachCharacter(Character? character, string userName)
        {
            this.character = character;
            ui.SetCharacter(character, userName);
        }

        private Character? character;

        public Character? GetCharacter()
        {
            AttachCharacter(character, UserName);
            return character;
        }

        public Person GetPerson()
        {
            return character?.Personality?.Person ?? Person.Friend;
        }
        #endregion

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

        public void Win()
        {
            ui.Win();
        }

        public void Lose()
        {
            ui.Lose();
        }

        public void PlayerFailed(IPlayer player)
        {
            ui.PlayerFailed(player.UserName);
        }

        public async Task<string> WaitForSelectPlayer()
        {
            return await ui.SelectPlayer();
        }

        public async Task<Character> WaitForSelectCharacter(Func<Character, bool> predicate)
        {
            while (true)
            {
                var character = await ui.SelectCharacter();

                if (predicate(character))
                    return character;
                else
                    ui.ShowError(Error.WrongCharacter().MessageString);
            }
        }

        public async Task<CardColor> WaitForSelectColor(CardColor[] colors)
        {
            return await ui.SelectColor(colors);
        }

        public void ShowCharacterPerson(Character character)
        {
            ui.ShowCharacterPerson(character);
        }

        public void AttachNextActionCard(ActionCard card)
        {
            next = card;
        }
    }
}
