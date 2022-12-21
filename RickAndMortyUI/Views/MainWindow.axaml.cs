using Avalonia.Controls;
using Avalonia;
using System.IO;
using System;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Styling;
using System.Threading.Tasks;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.Common.Game;
using RickAndMortyUI.ViewModels;
using Avalonia.Data;
using Avalonia.Controls.Shapes;
using Avalonia.Animation;
using Avalonia.Layout;
using System.Security.Cryptography;
using System.Threading;
using Avalonia.Platform;
using System.Collections.Generic;
using System.Linq;
using RickAndMortyLibrary;
using RickAndMortyLibrary.Test;
using Avalonia.Threading;

namespace RickAndMortyUI.Views
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel model;
        private IAssetLoader assets;

        private Thread mainThread;

        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += (s, e) =>
            {
                model = (MainWindowViewModel)DataContext;
                model.menuGridVM.Appear();

                model.gameGridVM.PlayerIconVMs = model.waitGridVM.IconVMs;
                model.gameGridVM.CharactersPanel.MainGrid = charactersGrid;
                model.gameGridVM.HandPanel.MainGrid = handGrid;
            };

            assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            menuGrid.IsVisible = true;

            mainThread = Thread.CurrentThread;

            createButton.Click += (s, e) => Task.Run(CreateClicked);
            joinButton.Click += (s, e) => Task.Run(JoinClicked);

            //Task.Run(Test);
        }

        public void Invoke(Action action)
        {
            Dispatcher.UIThread.Post(action);
        }

        public int[] GetRandomIds()
        {
            var enumerable = Enumerable.Range(0, 5).ToArray();

            var random = new Random();

            for (int i = 0; i < enumerable.Length; i++)
            {
                enumerable.Swap(i, random.Next(0, enumerable.Length));
            }

            return enumerable;
        }

        private CancellationTokenSource startGame;

        public void GoToWaitScreen(bool counter)
        {
            model.menuGridVM.Disappear();
            model.waitGridVM.Appear();

            var wait = model.waitGridVM;

            startGame = new CancellationTokenSource();
            if (counter)
                Task.Run(() => wait.StartCounting(10, startGame));
            Task.Run(() => wait.StartAnimation(startGame.Token));
        }

        public void GoToGameScreen()
        {
            model.waitGridVM.Disappear();
            model.gameGridVM.Appear();
        }

        public IImage GetImage(string fileName)
        {
            return new Bitmap(assets.Open(new Uri($"avares://RickAndMortyUI/Assets/{fileName}")));
        }

        //-----------------------------------------------------
        public int Id { get; set; }

        #region Creating game
        private Server server;

        public async Task CreateClicked()
        {
            if (!model.menuGridVM.ValidateInputs())
                return;

            GoToWaitScreen(true);

            await CreateGame();

            GoToGameScreen();
        }

        public async Task CreateGame()
        {
            GetInputs(out var ip, out var port);
            server = new Server(ip, port);
            
            await CheckForJoinPlayers(2);
        }

        public async Task CheckForJoinPlayers(int minCount)
        {
            var ids = GetRandomIds();
            ShowAndBindPlayer(ids[0]);

            for (int i = 1; i < 5; i++)
            {
                var client = await server.AwaitJoining(startGame.Token);
                if (startGame.IsCancellationRequested && i < minCount)
                {
                    startGame = new CancellationTokenSource();
                }
                else if (startGame.IsCancellationRequested || model.waitGridVM.CountingEnded)
                {
                    await server.BroadcastMessage("player stop");
                }

                if (client != null)
                {
                    await Task.Delay(500);

                    await server.BroadcastMessage("player " + ids[i].ToString());

                    foreach (var player in model.waitGridVM.IconVMs)
                    {
                        if (player.Id >= 0)
                            await client.SendMessage("player " + player.Id);
                    }

                    ShowAndBindPlayer(ids[i]);

                    Task.Run(() => model.waitGridVM.ShowText("Приветствуем игрока!", 3000));
                }
                else
                {
                    return;
                }
            }
        }
        #endregion

        #region Joining to game
        private Client client;

        public async Task JoinClicked()
        {
            if (!model.menuGridVM.ValidateInputs())
                return;

            try
            {
                await JoinToGame();
            }
            catch
            {
                model.menuGridVM.ErrorText = "Ошибка подключения";
                return;
            }

            GoToWaitScreen(false);

            await CheckForNewPlayers();

            //Task.Run(() => model.waitGridVM.ShowText("ok", 10000));
            startGame.Cancel();

            GoToGameScreen();
        }

        public async Task JoinToGame()
        {
            GetInputs(out var ip, out var port);
            client = new Client(ip, port);
        }

        public async Task CheckForNewPlayers()
        {
            for (int i = 0; i < 5; i++)
            {
                var msg = await client.WaitForMessage("player");

                if (msg == null)
                {
                    i--;
                    continue;
                }

                var id = msg.Split()[1];
                if (id == "stop")
                    break;

                ShowAndBindPlayer(int.Parse(id));

                Task.Run(() => model.waitGridVM.ShowText("Приветствуем игрока!", 3000));
            }
        }

        public async Task StartReceiveMessages()
        {

        }
        #endregion
        //------------------------------------------------------

        #region Common
        public void GetInputs(out string ip, out int port)
        {
            var menu = model.menuGridVM;

            ip = menu.IpText;
            port = int.Parse(menu.PortText);
        }

        public void ShowAndBindPlayer(int id)
        {
            var wait = model.waitGridVM;
            var index = wait.GetEmptyPlayer();
            
            if (index >= 0)
            {
                var image = GetImage($"user{id + 1}.png");
                wait.IconVMs[index].Id = id;
                wait.IconVMs[index].Source = image;
                wait.IconVMs[index].IsVisible = true;
            }
        }

        public async Task<StringMessage> ProcessMessage(StringMessage message)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
