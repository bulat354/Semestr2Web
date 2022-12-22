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
using static RickAndMortyUI.Controls.WaitPanel;

namespace RickAndMortyUI.Views
{
    public partial class MainWindow : Window, IPlayerController
    {
        private MainWindowViewModel model;
        private IAssetLoader assets;

        private CancellationTokenSource waiting;

        private MenuVM menuVM => model.menuGridVM;
        private WaitVM waitVM => model.waitGridVM;
        private GameVM gameVM => model.gameGridVM;

        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += (s, e) =>
            {
                model = (MainWindowViewModel)DataContext;
                menuVM.Appear();

                gameVM.PlayerIconVMs = waitVM.IconVMs;
                gameVM.CharactersPanel.MainGrid = charactersGrid;
                gameVM.HandPanel.MainGrid = handGrid;

                character.PointerEnter += gameVM.OnPointerEnter;
                character.PointerLeave += gameVM.OnPointerLeave;
                character1.PointerEnter += gameVM.OnPointerEnter;
                character1.PointerLeave += gameVM.OnPointerLeave;
                character2.PointerEnter += gameVM.OnPointerEnter;
                character2.PointerLeave += gameVM.OnPointerLeave;
                character3.PointerEnter += gameVM.OnPointerEnter;
                character3.PointerLeave += gameVM.OnPointerLeave;
                character4.PointerEnter += gameVM.OnPointerEnter;
                character4.PointerLeave += gameVM.OnPointerLeave;
                person.PointerEnter += gameVM.OnPointerEnter;
                person.PointerLeave += gameVM.OnPointerLeave;
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
            //if (!menuVM.ValidateInputs())
            //    return;

            GoToWaitScreen(true);

            //GetInputs(out var ip, out var port);
            var ip = "127.0.0.1";
            var port = 8888;
            server = new Server(ip, port);

            await CheckForJoinPlayers(2);

            GoToGameScreen();

            var game = new GameController();

            game.Game = gameVM;
            game.PlayerControllers = server.Clients
                .Select(x => (IPlayerController)new RemotePlayerController(x, x.Id))
                .Append(this).ToArray();
            game.IsAdvancedMode = true;

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
                else if (waiting.IsCancellationRequested || waitVM.CountingEnded)
                {
                    await server.BroadcastMessage(StringMessage.Create(MessageFirstGoal.Player, null, MessageSecondGoal.Stop));
                }

                if (client != null)
                {
                    await Task.Delay(500);

                    await server.BroadcastMessage(StringMessage.Create(MessageFirstGoal.Player, ids[i].ToString()));

                    foreach (var player in waitVM.IconVMs)
                    {
                        if (player.Id >= 0)
                            await client.SendMessage(StringMessage.Create(MessageFirstGoal.Player, player.Id.ToString()));
                    }

                    await client.SendMessage(StringMessage.Create(MessageFirstGoal.Timer, waitVM.Counter));
                    client.Id = ids[i];

                    ShowAndBindPlayer(ids[i]);
                    Task.Run(() => waitVM.ShowText("Приветствуем игрока!", 3000));
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
            //if (!menuVM.ValidateInputs())
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
                menuVM.ErrorText = "Ошибка подключения";
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
                    Task.Run(() => waitVM.StartCounting(msg.ToInt(), waiting));
                    continue;
                }
                if (msg.SecondGoal == MessageSecondGoal.Stop)
                    break;

                ShowAndBindPlayer(msg.ToInt());

                Task.Run(() => waitVM.ShowText("Новый игрок!", 3000));
            }
        }

        /// <summary>
        /// Wait for messages from server and process them
        /// </summary>
        public async Task StartReceiveMessages()
        {
            while (true)
            {
                var message = await client.WaitForAny();
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
            var menu = menuVM;

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
            menuVM.Disappear();

            waitVM.Reset();
            waitVM.Appear();

            var wait = waitVM;

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
            waitVM.Disappear();

            gameVM.Reset();
            gameVM.Appear();
        }

        /// <summary>
        /// Going to main menu screen after game
        /// </summary>
        public void GoToMainScreen()
        {
            gameVM.Disappear();

            menuVM.Reset();
            menuVM.Appear();
        }

        /// <summary>
        /// Add player to wait screen
        /// </summary>
        public void ShowAndBindPlayer(int id)
        {
            var wait = waitVM;
            var index = wait.GetEmptyPlayer();
            
            if (index >= 0)
            {
                var image = GetImage($"user{id + 1}.png");
                wait.IconVMs[index].Id = id;
                wait.IconVMs[index].Source = image;
                wait.IconVMs[index].IsVisible = true;
            }
        }

        public int Id => waitVM.IconVMs[2].Id;

        private Dictionary<int, Control> characterControls = new Dictionary<int, Control>();
        private List<Tuple<int, Control>> handControls = new List<Tuple<int, Control>>();

        /// <summary>
        /// Process message from server and send response if necessary
        /// </summary>
        public async Task<StringMessage?> ProcessMessage(StringMessage message)
        {
            switch (message.FirstGoal)
            {
                case (MessageFirstGoal.Character):
                    return await ProcessCharacter(message);
                case (MessageFirstGoal.Action):
                    return await ProcessAction(message);
                case (MessageFirstGoal.Person):
                    return await ProcessPerson(message);
            }

            return null;
        }

        public async Task<StringMessage?> ProcessCharacter(StringMessage message)
        {
            switch (message.SecondGoal)
            {
                case (MessageSecondGoal.Add):
                    await ProcessCharacterAdd(message);
                    break;
                case (MessageSecondGoal.Remove):
                    await ProcessCharacterRemove(message);
                    break;
                case (MessageSecondGoal.Attach):
                    await ProcessCharacterAttach(message);
                    break;
                case (MessageSecondGoal.Detach):
                    await ProcessCharacterDetach(message);
                    break;
            }

            return null;
        }

        private async Task ProcessCharacterAdd(StringMessage message)
        {
            var card = CardsImporter.GetCard<CharacterCard>(message.ToInt());
            var image = GetImage(card.ImagePath);
            var control = await gameVM.CharactersPanel.AddCharacter(image);
            characterControls.Add(card.Id, control);
        }

        private async Task ProcessCharacterRemove(StringMessage message)
        {
            var id = message.ToInt();
            var control = characterControls[id];
            gameVM.CharactersPanel.RemoveCharacter(control);
            characterControls.Remove(id);
        }

        private async Task ProcessCharacterAttach(StringMessage message)
        {
            var split = message.Message.Split();
            var cardId = int.Parse(split[0]);
            var playerId = int.Parse(split[1]);
            var index = gameVM.PlayerIconVMs.FirstIndex(x => x.Id == playerId);
            var card = CardsImporter.GetCard<CharacterCard>(cardId);
            var image = GetImage(card.ImagePath);

            gameVM.PlayerCharaterVms[index].Bind(image);
            gameVM.PlayerCharaterVms[index].Show();
        }

        private async Task ProcessCharacterDetach(StringMessage message)
        {
            var index = gameVM.PlayerIconVMs.FirstIndex(x => x.Id == message.ToInt());
            gameVM.PlayerCharaterVms[index].Hide();
        }

        public async Task<StringMessage?> ProcessAction(StringMessage message)
        {
            switch (message.SecondGoal)
            {
                case (MessageSecondGoal.Add):
                    await ProcessActionAdd(message);
                    break;
                case (MessageSecondGoal.Remove):
                    await ProcessActionRemove(message);
                    break;
            }

            return null;
        }

        private async Task ProcessActionAdd(StringMessage message)
        {
            var id = message.ToInt();
            var card = CardsImporter.GetCard<ActionCard>(id);
            var image = GetImage(card.ImagePath);
            var control = await gameVM.HandPanel.AddAction(image);
            handControls.Add(Tuple.Create(id, control));
        }

        private async Task ProcessActionRemove(StringMessage message)
        {
            var id = message.ToInt();
            var tuple = handControls.First(x => x.Item1 == id);
            gameVM.HandPanel.RemoveAction(tuple.Item2);
            handControls.Remove(tuple);
        }

        public async Task<StringMessage?> ProcessPerson(StringMessage message)
        {
            switch (message.SecondGoal)
            {
                case (MessageSecondGoal.Attach):
                    await ProcessPersonAttach(message);
                    break;
                case (MessageSecondGoal.Detach):
                    await ProcessPersonDetach(message);
                    break;
            }

            return null;
        }

        private async Task ProcessPersonAttach(StringMessage message)
        {
            var id = message.ToInt();
            var card = CardsImporter.GetCard<PersonalityCard>(id);
            var image = GetImage(card.ImagePath);

            gameVM.PersonVM.Bind(image);
            gameVM.PersonVM.Show();
        }

        private async Task ProcessPersonDetach(StringMessage message)
        {
            gameVM.PersonVM.Hide();
        }
        #endregion
    }
}
