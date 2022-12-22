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
    public partial class MainWindow : Window, IPlayerController
    {
        private MainWindowViewModel model;
        private IAssetLoader assets;

        private CancellationTokenSource waiting;

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

            createButton.Click += (s, e) => Task.Run(CreateClicked);
            joinButton.Click += (s, e) => Task.Run(JoinClicked);
        }

        // -----------------------------------------------Server-----------------------------------------------
        #region Server Side
        private Server server;

        /// <summary>
        /// When player click to Create
        /// </summary>
        /// <returns></returns>
        public async Task CreateClicked()
        {
            //if (!model.menuGridVM.ValidateInputs())
            //    return;

            GoToWaitScreen(true);

            //GetInputs(out var ip, out var port);
            var ip = "127.0.0.1";
            var port = 8888;
            server = new Server(ip, port);

            await CheckForJoinPlayers(2);

            GoToGameScreen();

            var game = new GameController();
            game.Game = model.gameGridVM;
            game.PlayerControllers = server.Clients
                .Select(x => (IPlayerController)new RemotePlayerController(x))
                .Append(this).ToArray();

            await game.Start();
        }

        /// <summary>
        /// Waiting for players connecting
        /// </summary>
        /// <param name="minCount">Minimal number of players to start game</param>
        public async Task CheckForJoinPlayers(int minCount)
        {
            var ids = GenerateIDs();
            ShowAndBindPlayer(ids[0]);

            for (int i = 1; i < 5; i++)
            {
                var client = await server.AwaitJoining(waiting.Token);
                if (waiting.IsCancellationRequested && i < minCount)
                {
                    waiting = new CancellationTokenSource();
                }
                else if (waiting.IsCancellationRequested || model.waitGridVM.CountingEnded)
                {
                    await server.BroadcastMessage(StringMessage.Create(MessageFirstGoal.Player, null, MessageSecondGoal.Stop));
                }

                if (client != null)
                {
                    await Task.Delay(500);

                    await server.BroadcastMessage(StringMessage.Create(MessageFirstGoal.Player, ids[i].ToString()));

                    foreach (var player in model.waitGridVM.IconVMs)
                    {
                        if (player.Id >= 0)
                            await client.SendMessage(StringMessage.Create(MessageFirstGoal.Player, player.Id.ToString()));
                    }

                    await client.SendMessage(StringMessage.Create(MessageFirstGoal.Timer, model.waitGridVM.Counter));

                    ShowAndBindPlayer(ids[i]);
                    Task.Run(() => model.waitGridVM.ShowText("Приветствуем игрока!", 3000));
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Generate random ids for players. Used by host.
        /// </summary>
        public int[] GenerateIDs()
        {
            var enumerable = Enumerable.Range(0, 5).ToArray();

            var random = new Random();

            for (int i = 0; i < enumerable.Length; i++)
            {
                enumerable.Swap(i, random.Next(0, enumerable.Length));
            }

            return enumerable;
        }
        #endregion

        // -----------------------------------------------Client-----------------------------------------------
        #region Joining to game
        private Client client;

        /// <summary>
        /// When player click to Join
        /// </summary>
        public async Task JoinClicked()
        {
            //if (!model.menuGridVM.ValidateInputs())
            //    return;

            try
            {
                //GetInputs(out var ip, out var port);
                var ip = "127.0.0.1";
                var port = 8888;
                client = new Client(ip, port);
            }
            catch
            {
                model.menuGridVM.ErrorText = "Ошибка подключения";
                return;
            }

            GoToWaitScreen(false);
            await CheckForNewPlayers();
            waiting.Cancel();

            GoToGameScreen();
            await StartReceiveMessages();
        }

        /// <summary>
        /// Wait for new players and add them to wait screen 
        /// </summary>
        public async Task CheckForNewPlayers()
        {
            for (int i = 0; i < 5; i++)
            {
                var msg = await client.WaitForMessage(MessageFirstGoal.Player | MessageFirstGoal.Timer, MessageSecondGoal.None | MessageSecondGoal.Stop);

                if (msg == null)
                {
                    i--;
                    continue;
                }

                if (msg.FirstGoal == MessageFirstGoal.Timer)
                {
                    Task.Run(() => model.waitGridVM.StartCounting(msg.ToInt(), waiting));
                    continue;
                }
                if (msg.SecondGoal == MessageSecondGoal.Stop)
                    break;

                ShowAndBindPlayer(msg.ToInt());

                Task.Run(() => model.waitGridVM.ShowText("Новый игрок!", 3000));
            }
        }

        /// <summary>
        /// Wait for messages from server and process them
        /// </summary>
        public async Task StartReceiveMessages()
        {
            while (true)
            {
                var message = await client.WaitForMessage(MessageFirstGoal.Any, MessageSecondGoal.Any);
                var response = await ProcessMessage(message);
                if (response != null)
                    await client.SendMessage(response);
            }
        }
        #endregion

        // -----------------------------------------------Common-----------------------------------------------
        #region Common
        /// <summary>
        /// Get ip and port inputs from menu screen
        /// </summary>
        public void GetInputs(out string ip, out int port)
        {
            var menu = model.menuGridVM;

            ip = menu.IpText;
            port = int.Parse(menu.PortText);
        }

        /// <summary>
        /// Load image from folder assets
        /// </summary>
        public IImage GetImage(string fileName)
        {
            return new Bitmap(assets.Open(new Uri($"avares://RickAndMortyUI/Assets/{fileName}")));
        }

        /// <summary>
        /// Going to waiting for players screen after main menu
        /// </summary>
        public void GoToWaitScreen(bool toStartTimer)
        {
            model.menuGridVM.Disappear();

            model.waitGridVM.Reset();
            model.waitGridVM.Appear();

            var wait = model.waitGridVM;

            waiting = new CancellationTokenSource();

            if (toStartTimer)
                Task.Run(() => wait.StartCounting(10, waiting));
            Task.Run(() => wait.StartAnimation(waiting.Token));
        }

        /// <summary>
        /// Going to main game screen after waiting for players
        /// </summary>
        public void GoToGameScreen()
        {
            model.waitGridVM.Disappear();

            model.gameGridVM.Reset();
            model.gameGridVM.Appear();
        }

        /// <summary>
        /// Going to main menu screen after game
        /// </summary>
        public void GoToMainScreen()
        {
            model.gameGridVM.Disappear();

            model.menuGridVM.Reset();
            model.menuGridVM.Appear();
        }

        /// <summary>
        /// Add player to wait screen
        /// </summary>
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

        private List<Control> characterControls = new List<Control>();

        /// <summary>
        /// Process message from server and send response if necessary
        /// </summary>
        public async Task<StringMessage?> ProcessMessage(StringMessage message)
        {
            switch (message.FirstGoal)
            {
                case (MessageFirstGoal.Character):
                    return await ProcessCharacter(message);
            }

            return null;
        }

        public async Task<StringMessage?> ProcessCharacter(StringMessage message)
        {
            switch (message.SecondGoal)
            {
                case (MessageSecondGoal.Add):
                    var card = CardsImporter.GetCard<CharacterCard>(message.ToInt());
                    var image = GetImage(card.ImagePath);
                    var control = await model.gameGridVM.CharactersPanel.AddCharacter(image);
                    characterControls.Add(control);
                    break;
            }

            return null;
        }
        #endregion
    }
}
