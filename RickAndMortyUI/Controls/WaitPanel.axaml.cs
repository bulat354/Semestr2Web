using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System.ComponentModel;
using System.Linq;

namespace RickAndMortyUI.Controls
{
    public class WaitPanel : TemplatedControl
    {
        //WaitText
        //DotsCount
        //TimeCounter
        //Player0-4Opacity
        //Player0-4Source

        public static readonly StyledProperty<string> WaitTextProperty =
            AvaloniaProperty.Register<WaitPanel, string>(nameof(WaitText));

        public string WaitText
        {
            get => GetValue(WaitTextProperty);
            set => SetValue(WaitTextProperty, value);
        }

        public static readonly StyledProperty<int> DotsCountProperty =
            AvaloniaProperty.Register<WaitPanel, int>(nameof(DotsCount));

        public int DotsCount
        {
            get => GetValue(DotsCountProperty);
            set 
            { 
                SetValue(DotsCountProperty, value); 
                WaitText = "ќжидание игроков" + new string('.', value); 
            } 
        }

        public static readonly StyledProperty<string> TimerCounterProperty =
            AvaloniaProperty.Register<WaitPanel, string>(nameof(TimerCounter));

        public string TimerCounter
        {
            get => GetValue(TimerCounterProperty);
            set => SetValue(TimerCounterProperty, value);
        }

        public Player GetPlayer(int index) => Players[index];
        public void SetPlayer(int index, Player player)
        {
            Players[index] = player;
        }

        public Player[] Players { get; set; } = new Player[]
        {
            new Player(),
            new Player(),
            new Player(),
            new Player(),
            new Player()
        };

        public struct Player
        {
            public double Opacity { get; set; }
            public IImage Image { get; set; }

            public Player()
            {
                Opacity = 0;
                Image = null;
            }
        }
    }
}
