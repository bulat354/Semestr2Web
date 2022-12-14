using RickAndMortyLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Local
{
    internal class NormalGame : GameBase
    {
        protected CancellationTokenSource stopGame;
        protected CancellationTokenSource stopWaitingVoting;

        public async override void StartGame()
        {
            // токен для остановки игры
            stopGame = new CancellationTokenSource();

            // перемешаем карты
            actionCardsPack.Shuffle();
            characterCardsPack.Shuffle();
            personalityCardsPack.Shuffle();

            // раздадим карты действий игрокам
            HandOutActionCards();
            // положим на стол персонажей
            LayCharacters();
            // перемешаем игроков
            ShufflePlayers();

            // на другом потоке запустим ожидание голосования
            WaitForVoting();

            var firstPlayer = 0;
            // пока игру не остановили
            while (stopGame.IsCancellationRequested)
            {
                // запускаем таймер на 15 секунд
                _players.ForEach(x => x.StartTimer(15));
                await Task.Delay(TimeSpan.FromSeconds(15), stopGame.Token);
                // и останавливаем, если игра остановлена
                if (stopGame.IsCancellationRequested)
                {
                    _players.ForEach(x => x.StopTimer());
                    break;
                }

                // если на столе остались персонажи
                if (characters.Count > 0)
                {
                    // начинаем раунд
                    if (characterCardsPack.Count() > 0)
                        await StartRound(firstPlayer, false);
                    else
                    {
                        // играем последний раунд и заканчиваем игру, если персонажей не осталось
                        await StartRound(firstPlayer, true);
                        break;
                    }
                }
                else
                    break;

                firstPlayer++;
            }

            stopWaitingVoting.Cancel();
            // объявляем победителей
            CheckForWinners();
        }

        // добавляет по три карты действия игрокам
        protected void HandOutActionCards()
        {
            for (int i = 0; i < 3; i++)
                foreach (var player in _players)
                {
                    player.AddCard(PopCard());
                }
        }

        // кладет на стол персонажей
        protected virtual void LayCharacters()
        {
            var count = _players.Length * 2;

            for (int i = 0; i < count; i++)
            {
                AddCharacterToTable();
            }
        }

        // кладет на стол нового персонажа
        protected void AddCharacterToTable()
        {
            var character = new Character()
            {
                Card = characterCardsPack.Pop(),
                Personality = personalityCardsPack.Pop()
            };
            // сообщает всем о добавлении персонажа
            _players.ForEach(p => p.AddCharacter(character));
        }

        // перемешивает игроков
        protected void ShufflePlayers()
        {
            var random = new Random();
            for (int i = 0; i < _players.Length; i++)
            {
                var num = random.Next(0, _players.Length);

                var player = _players[i];
                _players[i] = _players[num];
                _players[num] = player;
            }
        }

        // берет карту из колоды и, если карт не осталось, сброс делает колодой и перемешивает
        protected ActionCard PopCard()
        {
            if (actionCardsPack.Count() == 0)
            {
                actionCardsPack.Init(discardPile);
                discardPile.Clear();
            }

            return actionCardsPack.Pop();
        }

        // объявляет победителей
        protected virtual void CheckForWinners()
        { 
            if (characters.Any(x => IsCharacterParasite(x)))
            {
                _players.ForEach(x => x.Lose());
            }
            else
            {
                _players.ForEach(x => x.Win());
            }
            GameOverEvent.SetResult();
        }

        // проверяет персонажа на паразита
        protected bool IsCharacterParasite(Character character)
        {
            throw new NotImplementedException();
        }

        // начинает раунд
        protected virtual async Task StartRound(int firstPlayer, bool isFinal = false)
        {
            // ждет выбора карты действия для каждого игрока
            var tasks = _players.Select(x => x.WaitChoosingAction(stopGame.Token));
            await Task.WhenAll(tasks);
            if (stopGame.IsCancellationRequested)
                return;
            // берет их карты
            var cards = tasks.Select(x => x.Result).ToArray();
            if (!isFinal)
                // добавляет новые карты
                _players.ForEach(x => x.AddCard(PopCard()));

            for (int i = 0; i < _players.Length; i++)
            {
                if (stopGame.IsCancellationRequested)
                    return;
                // выполняет действие для каждой карты
                var number = (firstPlayer + i) % _players.Length;
                await Invoke(cards[number], _players[number]);
            }

            if (!isFinal)
                // добавляем персонажа на стол
                AddCharacterToTable();
        }

        // ожидает голосования
        protected async Task WaitForVoting()
        {
            while (true)
            {
                if (stopWaitingVoting == null)
                    stopWaitingVoting = new CancellationTokenSource();
                else
                    stopWaitingVoting.TryReset();

                // ждет пока один из игроков не нажмет на кнопку голосования
                await Task.WhenAny(_players.Select(x => x.WaitForVote(stopWaitingVoting.Token)));
                stopWaitingVoting.Cancel(false);

                // начинаем голосовать
                _players.ForEach(p => p.StartVoting());

                // голосуем и подсчитываем количество голосов
                var tasks = _players.Select(x => x.WaitVoteResult());
                var count = (await Task.WhenAll(tasks)).Where(x => x).Count();

                // останавливаем голосование
                _players.ForEach(x => x.StopVoting());

                // если голосов больше половины
                if (count > _players.Length / 2)
                {
                    // останавливаем игру
                    stopGame.Cancel(false);
                    break;
                }
            }
        }

        // выполняет действие из карты действия
        protected virtual Task Invoke(ActionCard action, IPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
