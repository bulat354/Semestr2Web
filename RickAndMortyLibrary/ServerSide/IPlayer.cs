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
    /// Интерфейс игрока. Используется для ожидания действий игрока и отправки результатов действий игроков.
    /// </summary>
    internal interface IPlayer
    {
        string UserName { get; set; }
        int Number { get; set; }

        void SendError(Error error);

        void SendNewPlayer(IPlayer player);
        void SendRemovePlayer(IPlayer player);

        void TakeCard(ActionCard actionCard);
        void AddCharacter(Character character);
        void RemoveCharacter(Character character, int timeOut = 0);

        Task WaitForVote(CancellationToken stopWaiting);

        void StartVoting();
        Task<bool> WaitVoteResult();
        void StopVoting();

        void StartTimer(int seconds);
        void StopTimer();

        Task<ActionCard?> WaitChoosingAction(CancellationToken stopWaiting);

        void Win();
        void Lose();

        void PlayerFailed(IPlayer player);

        #region Advanced game features
        Character GetCharacter();
        void AttachCharacter(Character character, string userName);
        Person GetPerson();
        #endregion

        #region Card invoking methods
        Task<string> WaitForSelectPlayer();
        Task<Character> WaitForSelectCharacter(Func<Character, bool> predicate);
        Task<CardColor> WaitForSelectColor(CardColor[] colors);

        void ShowTopFromPack(PersonalityCard card);
        void ShowCharacterPerson(Character character);

        void AttachNextActionCard(ActionCard card);
        #endregion
    }
}
