using Avalonia.Controls;
using Avalonia;
using System.IO;
using System;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Styling;
using RickAndMortyLibrary.Common;
using System.Threading.Tasks;
using RickAndMortyLibrary.ServerSide.Game;
using RickAndMortyLibrary.Common.Game.Cards;
using RickAndMortyLibrary.ServerSide;
using RickAndMortyLibrary.Common.Game;
using RickAndMortyUI.ViewModels;
using Avalonia.Data;
using Avalonia.Controls.Shapes;
using Avalonia.Animation;
using Avalonia.Layout;
using System.Security.Cryptography;
using System.Threading;
using Avalonia.Platform;

namespace RickAndMortyUI.Views
{
    public partial class MainWindow : Window, IMainUI, IPlayerUI
    {
        private string imagesPath;

        private MainWindowViewModel model;
        private StackPanel mainPanel;

        private TextBox ipInput;
        private TextBox portInput;
        private TextBox nameInput;

        private IAssetLoader assets;

        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += (s, e) =>
            {
                model = (MainWindowViewModel)DataContext;
                InitializeMainPanel();
            };

            assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        }

        public async void InitializeMainPanel()
        {
            await Task.Delay(500);

            model = (MainWindowViewModel)DataContext;
            model.Window = this;

            mainPanel = new StackPanel();
            mainPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            mainPanel.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            mainPanel.Spacing = 20;
            mainPanel.Bind(OpacityProperty, new Binding("MainVisibility"));
            mainPanel.Transitions = new Transitions
            {
                GetOpacityTransition(0.5)
            };

            var logo = new Image();
            logo.Stretch = Stretch.None;
            logo.Margin = new Thickness(0, 0, 0, 50);
            logo.Source = new Bitmap(assets.Open(new Uri("avares://RickAndMortyUI/Assets/logo.png")));
            mainPanel.Children.Add(logo);

            ipInput = new TextBox();
            ipInput.MaxWidth = 400;
            ipInput.Watermark = "Введите IP-адрес";
            mainPanel.Children.Add(ipInput);

            portInput = new TextBox();
            portInput.MaxWidth = 400;
            portInput.Watermark = "Введите порт";
            mainPanel.Children.Add(portInput);

            nameInput = new TextBox();
            nameInput.MaxWidth = 400;
            nameInput.Watermark = "Введите ник";
            mainPanel.Children.Add(nameInput);

            var buttonsPanel = new StackPanel();
            buttonsPanel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            buttonsPanel.Orientation = Avalonia.Layout.Orientation.Horizontal;
            buttonsPanel.Spacing = 50;
            buttonsPanel.Margin = new Thickness(0, 50);

            var joinButton = new Button();
            joinButton.Click += (s, e) => JoinClicked();
            joinButton.Content = new TextBlock() { Text = "Присоединиться" };
            buttonsPanel.Children.Add(joinButton);

            var createButton = new Button();
            createButton.Click += (s, e) => CreateClicked(false);
            createButton.Content = new TextBlock() { Text = "Создать" };
            buttonsPanel.Children.Add(createButton);

            var createAdvButton = new Button();
            createAdvButton.Click += (s, e) => CreateClicked(true);
            createAdvButton.Content = new TextBlock() { Text = "Создать (продвинутый режим)" };
            buttonsPanel.Children.Add(createAdvButton);

            mainPanel.Children.Add(buttonsPanel);

            Content = mainPanel;
            InvalidateVisual();

            model.MainVisibility = 1;
        }

        private StackPanel waitPanel;
        private CancellationTokenSource waiting;

        private Image[] playerImages;

        public async void InitializeWaitPanel()
        {
            await Task.Delay(500);

            model.StartWaitingTextAnim(1000, waiting);

            waitPanel = new StackPanel();
            waitPanel.Margin = new Thickness(0, 50);

            var logo = new Image();
            logo.Source = new Bitmap(assets.Open(new Uri("avares://RickAndMortyUI/Assets/logo.png")));
            logo.Width = 400;
            waitPanel.Children.Add(logo);

            var waitingText = new TextBlock();
            waitingText.Classes.Add("dimbo");
            waitingText.HorizontalAlignment = HorizontalAlignment.Center;
            waitingText.FontSize = 30;
            waitingText.Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            waitingText.Bind(TextBox.TextProperty, new Binding("WaitText"));
            waitPanel.Children.Add(waitingText);

            playerImages = new Image[5];
            for (int i = 0; i < 5; i++)
            {
                var player = new Image();
                player.Width = 100;
                player.Opacity = 0;
                player.Source = new Bitmap(assets.Open(new Uri($"avares://RickAndMortyUI/Assets/user{i + 1}.png")));
                playerImages[i] = player;
            }

            var countdownText = new TextBlock();
            countdownText.Width = 100;
            countdownText.Classes.Add("dimbo");
            countdownText.FontSize = 50;
            countdownText.Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            countdownText.TextAlignment = TextAlignment.Center;
            countdownText.VerticalAlignment = VerticalAlignment.Center;
            countdownText.Bind(TextBlock.TextProperty, new Binding("WaitTimer"));

            var panel1 = new StackPanel();
            panel1.HorizontalAlignment = HorizontalAlignment.Center;
            panel1.Spacing = 100;
            panel1.Margin = new Thickness(0, 25);
            panel1.Children.Add(playerImages[0]);
            panel1.Children.Add(playerImages[1]);
            panel1.Orientation = Orientation.Horizontal;
            waitPanel.Children.Add(panel1);

            var panel2 = new StackPanel();
            panel2.HorizontalAlignment = HorizontalAlignment.Center;
            panel2.Margin = new Thickness(0, 50);
            panel2.Spacing = 200;
            panel2.Children.Add(playerImages[2]);
            panel2.Children.Add(countdownText);
            panel2.Children.Add(playerImages[3]);
            panel2.Orientation = Orientation.Horizontal;
            waitPanel.Children.Add(panel2);

            playerImages[4].Margin = new Thickness(0, 25);
            playerImages[4].HorizontalAlignment = HorizontalAlignment.Center;
            waitPanel.Children.Add(playerImages[4]);

            Content = waitPanel;
            InvalidateVisual();

            waiting = new CancellationTokenSource();
            DataContextChanged += (s, e) => ((MainWindowViewModel)DataContext).StartWaitingTextAnim(1000, waiting);

            model.WaitVisibility = 1;
        }

        public DoubleTransition GetOpacityTransition(double durationSec, double delaySec = -1)
        {
            var trans = new DoubleTransition();
            if (durationSec > 0)
                trans.Duration = TimeSpan.FromSeconds(durationSec);
            if (delaySec > 0)
                trans.Delay = TimeSpan.FromSeconds(delaySec);
            trans.Property = OpacityProperty;

            return trans;
        }

        #region
        public void CreateClicked(bool isAdvanced)
        {
            waiting = new CancellationTokenSource();

            Test(waiting.Token);

            model.ToWaitScreen();
            InitializeWaitPanel();
        }

        public void JoinClicked()
        {
            waiting = new CancellationTokenSource();

            Test(waiting.Token);

            model.ToWaitScreen();
            InitializeWaitPanel();
        }

        public async void Test(CancellationToken token)
        {
            await Task.Delay(5000, token);

            playerImages[4].Opacity = 1;
            playerImages[4].InvalidateVisual();

            await Task.Delay(2000, token);

            playerImages[0].Opacity = 1;
            playerImages[0].InvalidateVisual();

            await Task.Delay(1000, token);

            playerImages[3].Opacity = 1;
            playerImages[3].InvalidateVisual();

            await Task.Delay(5000, token);

            playerImages[2].Opacity = 1;
            playerImages[2].InvalidateVisual();

            await Task.Delay(3000, token);

            playerImages[1].Opacity = 1;
            playerImages[1].InvalidateVisual();

            await Task.Delay(100000, token);

            model.WaitVisibility = 0;
            InitializeMainPanel();
        }
        #endregion
        public void AddCardToHand(ActionCard card)
        {
            throw new NotImplementedException();
        }

        public void AddCharacter(Character character)
        {
            throw new NotImplementedException();
        }

        public void AddPlayer(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<ActionCard> ChooseActionFromHand()
        {
            throw new NotImplementedException();
        }

        public Task<Character> GetCharacter()
        {
            throw new NotImplementedException();
        }

        public void Lose()
        {
            throw new NotImplementedException();
        }

        public void PlayerFailed(string playerName)
        {
            throw new NotImplementedException();
        }

        public void RemoveCharacter(Character character, int timeout)
        {
            throw new NotImplementedException();
        }

        public void RemovePlayer(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<Character> SelectCharacter()
        {
            throw new NotImplementedException();
        }

        public Task<CardColor> SelectColor(CardColor[] colors)
        {
            throw new NotImplementedException();
        }

        public Task<string> SelectPlayer()
        {
            throw new NotImplementedException();
        }

        public void SetCharacter(Character character, string userName)
        {
            throw new NotImplementedException();
        }

        public void ShowCharacterPerson(Character character)
        {
            throw new NotImplementedException();
        }

        public void ShowError(string error)
        {
            throw new NotImplementedException();
        }

        public void ShowTopFromPack(PersonalityCard card)
        {
            throw new NotImplementedException();
        }

        public void StartTimer(int sec)
        {
            throw new NotImplementedException();
        }

        public void StartVoting()
        {
            throw new NotImplementedException();
        }

        public void StopTimer()
        {
            throw new NotImplementedException();
        }

        public void StopVoting()
        {
            throw new NotImplementedException();
        }

        public Task WaitForPressStart()
        {
            throw new NotImplementedException();
        }

        public Task WaitForStartVoting()
        {
            throw new NotImplementedException();
        }

        public Task<bool> WaitForVotingResult()
        {
            throw new NotImplementedException();
        }

        public void Win()
        {
            throw new NotImplementedException();
        }

        public Task<IMainUI> ToMainScreen()
        {
            throw new NotImplementedException();
        }

        #region
        public Task<IPlayerUI> ToHostPlayerScreen()
        {
            throw new NotImplementedException();
        }
        public Task<IPlayerUI> ToLocalPlayerScreen()
        {
            throw new NotImplementedException();
        }

        public Task WaitForConnecting(out string ipAddress, out int port, out string userName)
        {
            throw new NotImplementedException();
        }
        public Task WaitForCreating(out string ipAddress, out int port, out GameType gameType, out string userName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
