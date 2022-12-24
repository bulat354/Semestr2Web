using RickAndMortyLibrary.Common.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ServerSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Common
{
    /// <summary>
    /// Интерфейс для интерфейса игрока.
    /// </summary>
    public interface IPlayerUI
    {
        Task<IMainUI> ToMainScreen();

        Task WaitForPressStart();
        void ShowError(string error);

        void AddPlayer(string userName);
        void RemovePlayer(string userName);

        void AddCardToHand(ActionCard card);
        void AddCharacter(Character character);

        Task WaitForStartVoting();
        Task<bool> WaitForVotingResult();

        void StartVoting();
        void StopVoting();

        void StartTimer(int sec);
        void StopTimer();

        void Win();
        void Lose();

        Task<ActionCard> ChooseActionFromHand();

        Task<Character> GetCharacter();
        void SetCharacter(Character character, string userName);

        void RemoveCharacter(Character character, int timeout);
        void PlayerFailed(string playerName);

        Task<string> SelectPlayer();
        Task<Character> SelectCharacter();
        Task<CardColor> SelectColor(CardColor[] colors);

        void ShowTopFromPack(PersonalityCard card);
        void ShowCharacterPerson(Character character);
    }
}
