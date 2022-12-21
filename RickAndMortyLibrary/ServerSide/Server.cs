using DPTPLibrary;
using RickAndMortyLibrary.Common;
using RickAndMortyLibrary.Messages;
using RickAndMortyLibrary.ServerSide.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.ServerSide
{
    public class Server : IServer
    {
        private DPTPListener listener;
        private List<IPlayer> players = new List<IPlayer>();
        private IMainUI ui;

        private CancellationTokenSource stopWaitingPlayers;

        public List<int> PlayerIds { get; set; }

        public async Task Start(IMainUI ui)
        {
            this.ui = ui;
            // ожидаем ввода айпи, порта, типа игры и имени
            await ui.WaitForCreating(out var ip, out var port, out var type, out var userName);

            // создаем листенер
            listener = new DPTPListener(ip, port);
            listener.Start();

            // переходим на окно ожидания
            var hostUI = await ui.ToHostPlayerScreen();

            // не забываем добавить хоста в список игроков
            var host = new HostPlayer(PlayerIds[PlayerIds.Count - 1].ToString(), hostUI);
            PlayerIds.RemoveAt(PlayerIds.Count - 1);
            AddPlayer(host);
            // ждем присоединения игроков
            await WaitForPlayers(type, host);

            /*
            // создаем игру, инициализируем и стартуем
            GameBase game = type == GameType.Normal ? new NormalGame() : new AdvancedGame();
            game.Init(players.ToArray());
            game.StartGame();

            // ожидаем окончания игры и отключаем игроков и сервер
            await game.GameOverEvent.Task;
            players
                .Where(p => p is RemotePlayer)
                .ForEach(p => ((RemotePlayer)p).Disconnect());
            listener.Stop();
            */
        }

        private async Task WaitForPlayers(GameType type, HostPlayer host)
        {
            // создаем токен для отмены ожидания игроков
            stopWaitingPlayers = new CancellationTokenSource();

            // минимальное и максимальное количество игроков на сервере
            var min = type == GameType.Normal ? 2 : 3;
            var max = type == GameType.Normal ? 5 : 6;

            // ожидаем готовности хоста
            PlayerReady(host, min);

            while (true)
            {
                // ожидаем присоединения игрока
                Client client;
                try
                {
                    client = new Client(await listener.AcceptClientAsync().WaitAsync(stopWaitingPlayers.Token));
                }
                catch
                {
                    break;
                }

                // если игроков уже максимальное число
                if (players.Count == max)
                {
                    // отправляем соответствующее сообщение
                    client.Send(Error.MaxPlayers());
                    // отключаем клиента
                    client.Disconnect();
                }
                else
                {
                    // создаем удаленного игрока
                    var player = new RemotePlayer(client) { UserName = PlayerIds[PlayerIds.Count - 1].ToString() };
                    PlayerIds.RemoveAt(PlayerIds.Count - 1);
                    player.SendNewPlayer(player);
                    player.StartReceiving();
                    // добавляем игрока
                    AddPlayer(player);
                    // ожидаем отсоединения игрока
                    PlayerDisconnect(player);
                }
            }
        }

        private void AddPlayer(IPlayer player)
        {
            lock (players)
            {
                // сообщаем другим игрокам, что присоединился новый
                players.ForEach(p => p.SendNewPlayer(player));
                // добавляем нового игрока
                players.Add(player);
            }
        }

        private async void PlayerDisconnect(RemotePlayer player)
        {
            // ожидаем отсоединения игрока до того момента, когда хост нажал на готов
            try
            {
                await player.Disconnected.Task.WaitAsync(stopWaitingPlayers.Token);
            }
            catch
            {
                return;
            }

            // удаляем игрока и сообщаем об этом другим
            lock (players)
            {
                players.ForEach(p => p.SendRemovePlayer(player));
                players.Remove(player);
            }

            PlayerIds.Add(int.Parse(player.UserName));
        }

        private async Task PlayerReady(HostPlayer player, int minCount)
        {
            while (true)
            {
                // ожидает готовности хоста
                await player.GetReady();
                lock (players)
                {
                    // если игроков недостаточно, отправляет соотв. сообщение и продолжает ожидание
                    if (players.Count < minCount)
                    {
                        player.SendError(Error.MinPlayers());
                        continue;
                    }
                }
                // если игроков в достатке, то объявляет, что больше игроков не ожидается
                stopWaitingPlayers.Cancel(false);
            }
        }
    }
}
