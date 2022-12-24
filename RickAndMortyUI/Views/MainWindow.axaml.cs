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
using System.Reflection;
using Avalonia.Interactivity;

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

        private List<VisualCard> playerCharacters;
        private List<VisualCard> tableCharacters;
        private List<VisualCard> playerIcons;

        private List<VisualCard> handCards;
        private VisualCard? AttachedAction;

        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += (s, e) =>
            {
                Init();
            };

            assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

            menuGrid.IsVisible = true;

            createButton.Click += (s, e) => Task.Run(() => CreateClicked(false));
            advancedButton.Click += (s, e) => Task.Run(() => CreateClicked(true));
            joinButton.Click += (s, e) => Task.Run(() => JoinClicked());
        }

        private void Init()
        {
            model = (MainWindowViewModel)DataContext;
            menuVM.Appear();
            //gameVM.Appear();

            gameVM.PlayerIconVMs = waitVM.IconVMs;
            gameVM.CharactersPanel.MainGrid = charactersGrid;
            gameVM.HandPanel.MainGrid = handGrid;

            #region
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
            #endregion

            playerIcons = new List<VisualCard>()
            {
                new VisualCard() { Control = gamePlayer1 },
                new VisualCard() { Control = gamePlayer2 },
                new VisualCard(),
                new VisualCard() { Control = gamePlayer3 },
                new VisualCard() { Control = gamePlayer4 }
            };

            playerCharacters = new List<VisualCard>()
            {
                new VisualCard() { Control = character1, Vm = gameVM.PlayerCharaterVms[0] },
                new VisualCard() { Control = character2, Vm = gameVM.PlayerCharaterVms[1] },
                new VisualCard() { Control = character, Vm = gameVM.PlayerCharaterVms[2] },
                new VisualCard() { Control = character3, Vm = gameVM.PlayerCharaterVms[3] },
                new VisualCard() { Control = character4 , Vm = gameVM.PlayerCharaterVms[4] }
            };
        }

        // -----------------------------------------------Server-----------------------------------------------
        #region Server Side
        private Server server;

        /// <summary>
        /// When player click to Create
        /// </summary>
        /// <returns></returns>

        public void CreateClicked(bool isAdvanced)
        {
            if (!menuVM.ValidateInputs())
                return;

            GoToWaitScreen(true);
            Dispatcher.UIThread.Post(() => Title = "Õîñò");

            GetInputs(out var ip, out var port);
            //var ip = "127.0.0.1";
            //var port = 8888;
            server = new Server(ip, port);

            CheckForJoinPlayers(isAdvanced ? 3 : 2);

            GoToGameScreen();

            var game = new GameController();

            game.Game = gameVM;
            game.PlayerControllers = server.Clients
                .Select(x => (IPlayerController)new RemotePlayerController(x, x.Id))
                .Append(this).ToArray();
            game.IsAdvancedMode = isAdvanced;

            game.Start();

            Thread.Sleep(3000);
        }

        /// <summary>
        /// Waiting for players connecting
        /// </summary>
        /// <param name="minCount">Minimal number of players to start game</param>
        public void CheckForJoinPlayers(int minCount)
        {
            var ids = GenerateIDs();
            ShowAndBindPlayer(ids[0]);

            var count = 1;
            while (true)
            {
                var client = server.AwaitJoining(waiting.Token);

                if (client != null)
                {
                    Thread.Sleep(500);

                    server.BroadcastMessage(StringMessage.Create(MessageFirstGoal.Player, ids[count].ToString()));

                    foreach (var player in waitVM.IconVMs)
                    {
                        if (player.Id >= 0)
                            client.SendMessage(StringMessage.Create(MessageFirstGoal.Player, player.Id.ToString()));
                    }

                    client.SendMessage(StringMessage.Create(MessageFirstGoal.Timer, waitVM.Counter));
                    client.Id = ids[count];

                    ShowAndBindPlayer(ids[count]);
                    Task.Run(() => waitVM.ShowText("Ïðèâåòñòâóåì èãðîêà!", 3000));

                    count++;
                }

                if (waiting.IsCancellationRequested && count < minCount)
                    waiting = new CancellationTokenSource();
                else if (waitVM.CountingEnded && count >= minCount)
                {
                    server.BroadcastMessage(StringMessage.Create(MessageFirstGoal.Player, null, MessageSecondGoal.Stop));
                    break;
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
        public void JoinClicked()
        {
            if (!menuVM.ValidateInputs())
                return;

            try
            {
                GetInputs(out var ip, out var port);
                //var ip = "127.0.0.1";
                //var port = 8888;
                client = new Client(ip, port);
            }
            catch
            {
                menuVM.ErrorText = "Îøèáêà ïîäêëþ÷åíèÿ";
                return;
            }

            GoToWaitScreen(false);
            Dispatcher.UIThread.Post(() => Title = "Êëèåíò");
            CheckForNewPlayers();
            waiting.Cancel();

            GoToGameScreen();
            StartReceiveMessages();

            Thread.Sleep(3000);
            GoToMainScreen();
        }

        /// <summary>
        /// Wait for new players and add them to wait screen 
        /// </summary>
        public void CheckForNewPlayers()
        {
            for (int i = 0; i < 5; i++)
            {
                var msg = client.WaitForMessage(MessageFirstGoal.Player | MessageFirstGoal.Timer, MessageSecondGoal.None | MessageSecondGoal.Stop);

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

                Task.Run(() => waitVM.ShowText("Íîâûé èãðîê!", 3000));
            }
        }

        /// <summary>
        /// Wait for messages from server and process them
        /// </summary>
        public void StartReceiveMessages()
        {
            while (true)
            {
                var message = client.WaitForAny();

                if (message != null)
                {
                    var response = ProcessMessage(message, true);
                    if (response != null)
                        client.SendMessage(response);
                }
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
                Task.Run(() => wait.StartCounting(30, waiting));
            Task.Run(() => wait.StartAnimation(waiting.Token));
        }

        /// <summary>
        /// Going to main game screen after waiting for players
        /// </summary>
        public void GoToGameScreen()
        {
            tableCharacters = new List<VisualCard>();
            handCards = new List<VisualCard>();

            waitVM.Disappear();

            gameVM.Reset();
            gameVM.Appear();
            PropertyChanged += OnWidthChanged;
        }

        public void OnWidthChanged(object? s, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Width")
                gameVM.CharactersPanel.OnWidthChanged();
        }

        /// <summary>
        /// Going to main menu screen after game
        /// </summary>
        public void GoToMainScreen()
        {
            PropertyChanged -= OnWidthChanged;
            gameVM.Disappear();

            menuVM.Reset();
            menuVM.Appear();

            Dispatcher.UIThread.Post(() => Title = "Главное меню");
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

                playerCharacters[index].PlayerId = id;
                playerIcons[index].PlayerId = id;
            }
        }

        public int Id => waitVM.IconVMs[2].Id;

        /// <summary>
        /// Process message from server and send response if necessary
        /// </summary>
        public StringMessage? ProcessMessage(StringMessage message, bool isRequest = false)
        {
            switch (message.FirstGoal)
            {
                case (MessageFirstGoal.Player):
                    return ProcessPlayer();
                case (MessageFirstGoal.Character):
                    return ProcessCharacter(message);
                case (MessageFirstGoal.Action):
                    return ProcessAction(message);
                case (MessageFirstGoal.Person):
                    ProcessPerson(message); break;
                case (MessageFirstGoal.Message):
                    ProcessMessage(message); break;
                case (MessageFirstGoal.Voting):
                    return ProcessVoting(message);
            }

            return null;
        }

        public StringMessage? ProcessPlayer()
        {
            var src = new TaskCompletionSource<int>();
            foreach (var item in playerIcons.Where(x => x.Control != null))
            {
                var icon = item;
                icon.MakeClickable();
                icon.Control.PointerPressed += (s, e) =>
                {
                    src.TrySetResult(icon.PlayerId);
                };
            }
            src.Task.Wait();
            playerIcons.Where(x => x.Control != null).ForEach(x => x.MakeUnclickable());

            return new StringMessage(MessageFirstGoal.Player, src.Task.Result.ToString());
        }

        public void ProcessMessage(StringMessage message)
        {
            var text = message.Message.Substring(0, Math.Min(45, message.Message.Length)) 
                + (message.Message.Length > 45 ? "..." : "");
            Task.Run(() => gameVM.ShowText(message.Message, message.SecondGoal == MessageSecondGoal.ForTime ? 3000 : -1));
        }

        public StringMessage? ProcessVoting(StringMessage message)
        {
            var src = new TaskCompletionSource<bool>();

            EventHandler<RoutedEventArgs> agree = (s, e) => src.SetResult(true);
            EventHandler<RoutedEventArgs> disagree = (s, e) => src.SetResult(false);

            agreeButton.Click += agree;
            disagreeButton.Click += disagree;

            gameVM.ShowVoting();

            src.Task.Wait();

            gameVM.HideVoting();

            agreeButton.Click -= agree;
            disagreeButton.Click -= disagree;

            return StringMessage.Create(MessageFirstGoal.None, src.Task.Result ? "yes" : "no");
        }

        public StringMessage? ProcessCharacter(StringMessage message)
        {
            switch (message.SecondGoal)
            {
                case MessageSecondGoal.Add:
                    ProcessCharacterAdd(message); break;
                case MessageSecondGoal.Remove:
                    ProcessCharacterRemove(message); break;
                case MessageSecondGoal.Attach:
                    ProcessCharacterAttach(message); break;
                case MessageSecondGoal.Detach:
                    ProcessCharacterDetach(message); break;
                case MessageSecondGoal.None:
                    return ProcessCharacterSelect(message);
            }

            return null;
        }

        private StringMessage? ProcessCharacterSelect(StringMessage message)
        {
            var ids = message.Message.Split(' ').Select(x => int.Parse(x)).ToArray();
            var cards = tableCharacters
                .Concat(playerCharacters)
                .Where(x => ids.Contains(x.Id))
                .ToArray();
            if (!cards.Any())
                return null;

            var src = new TaskCompletionSource<VisualCard>();
            foreach (var item in cards)
            {
                var card = item;
                card.MakeClickable();
                card.Control.PointerPressed += (s, e) =>
                {
                    src.TrySetResult(card);
                };
            }

            src.Task.Wait();
            foreach (var card in cards)
                card.MakeUnclickable();
            return new StringMessage(MessageFirstGoal.Character, src.Task.Result.Id.ToString());
        }

        private void ProcessCharacterDetach(StringMessage message)
        {
            var playerId = message.ToInt();

            var visual = playerCharacters.FirstOrDefault(x => x.PlayerId == playerId);
            visual.Id = -1;
            visual.Vm.Hide();
            visual.Vm.Source = null;
        }

        private void ProcessCharacterAttach(StringMessage message)
        {
            var split = message.Message.Split(' ').Select(x => int.Parse(x)).ToArray();
            
            var cardId = split[0];
            var card = CardsImporter.GetCard<CharacterCard>(cardId);
            var image = GetImage(card.ImagePath);
            var playerId = split[1];

            var visual = playerCharacters.First(x => x.PlayerId == playerId);
            visual.Id = cardId;
            visual.Vm.Bind(image);
            visual.Vm.Show();
        }

        private void ProcessCharacterRemove(StringMessage message)
        {
            var cardId = message.ToInt();
            var visual = tableCharacters.FirstOrDefault(x => x.Id == cardId);
            tableCharacters.Remove(visual);

            gameVM.CharactersPanel.RemoveCharacter(visual.Control);
        }

        private void ProcessCharacterAdd(StringMessage message)
        {
            var cardId = message.ToInt();
            var card = CardsImporter.GetCard<CharacterCard>(cardId);
            var image = GetImage(card.ImagePath);

            var vm = new CharacterVM() { CardId = cardId, IsVisible = true, Source = image };
            var control = gameVM.CharactersPanel.AddCharacter(vm);
            tableCharacters.Add(new VisualCard() { Control = control, Id = cardId, Vm = vm });
        }

        public StringMessage? ProcessAction(StringMessage message)
        {
            switch (message.SecondGoal)
            {
                case MessageSecondGoal.Add:
                    ProcessActionAdd(message); break;
                case MessageSecondGoal.Attach:
                    ProcessActionAttach(message); break;
                case MessageSecondGoal.Remove:
                    ProcessActionRemove(message); break;
                case MessageSecondGoal.None:
                    return ProcessActionSelect(message);
            }

            return null;
        }

        private void ProcessActionAttach(StringMessage message)
        {
            var cardId = message.ToInt();

            AttachedAction = new VisualCard() { Id = cardId };
        }

        private StringMessage? ProcessActionSelect(StringMessage message)
        {
            if (AttachedAction != null)
            {
                var msg = new StringMessage(MessageFirstGoal.Action, AttachedAction.Id.ToString());
                AttachedAction = null;
                return msg;
            }

            if (handCards.Count == 0)
                return null;

            var src = new TaskCompletionSource<VisualCard>();
            foreach (var item in handCards)
            {
                var card = item;
                card.MakeClickable();
                card.Control.PointerPressed += (s, e) =>
                {
                    src.TrySetResult(card);
                };
            }

            src.Task.Wait();
            foreach (var card in handCards)
                card.MakeUnclickable();

            handCards.Remove(src.Task.Result);
            gameVM.HandPanel.RemoveAction(src.Task.Result.Control);
            return new StringMessage(MessageFirstGoal.Character, src.Task.Result.Id.ToString());
        }

        private void ProcessActionRemove(StringMessage message)
        {
            throw new NotImplementedException();
        }

        private void ProcessActionAdd(StringMessage message)
        {
            var cardId = message.ToInt();
            var card = CardsImporter.GetCard<ActionCard>(cardId);
            var image = GetImage(card.ImagePath);

            var vm = new CharacterVM() { CardId = cardId, IsVisible = true, Source = image };
            var control = gameVM.HandPanel.AddAction(vm);
            handCards.Add(new VisualCard() { Control = control, Id = cardId, Vm = vm });
        }

        public StringMessage? ProcessPerson(StringMessage message)
        {
            switch (message.SecondGoal)
            {
                case MessageSecondGoal.Attach:
                    ProcessPersonAttach(message); break;
                case MessageSecondGoal.Detach:
                    ProcessPersonDetach(message); break;
                case MessageSecondGoal.Add:
                    ProcessPersonShow(message); break;
            }

            return null;
        }

        private void ProcessPersonShow(StringMessage message)
        {
            var split = message.Message.Split(' ').Select(x => int.Parse(x)).ToArray();
            var cardId = split[0];
            var persId = split[1];
            var card = CardsImporter.GetCard<PersonalityCard>(persId);
            var image = GetImage(card.ImagePath);

            var visual = tableCharacters.Concat(playerCharacters)
                .FirstOrDefault(x => x.Id == cardId);

            Task.Run(() =>
            {
                var oldImage = visual.Vm.Source;
                visual.Vm.Bind(image);
                visual.Vm.Show();
                Thread.Sleep(3000);
                if (visual != null)
                    visual.Vm.Bind(oldImage);
            });
        }

        private void ProcessPersonDetach(StringMessage message)
        {
            gameVM.PersonVM.Hide();
            gameVM.PersonVM.Source = null;
        }

        public void ProcessPersonAttach(StringMessage message)
        {
            var split = message.Message.Split(' ').Select(x => int.Parse(x)).ToArray();
            var id = split[0];
            var card = CardsImporter.GetCard<PersonalityCard>(id);
            var image = GetImage(card.ImagePath);

            if (split.Length == 1)
            {
                gameVM.PersonVM.Bind(image);
                gameVM.PersonVM.Show();
            }
            else
            {
                var playerId = split[1];

                var visual = playerCharacters.FirstOrDefault(x => x.PlayerId == playerId);
                visual.Vm.Bind(image);
                visual.Vm.Show();
            }
        }
        #endregion
    }
}
