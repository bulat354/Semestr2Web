using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace RickAndMortyUI.Controls
{
    public class MainPanel : TemplatedControl
    {
        public static readonly DirectProperty<MainPanel, string> IpTextProperty =
            AvaloniaProperty.RegisterDirect<MainPanel, string>(nameof(IpText), 
                o => o.IpText, 
                (o, x) => o.IpText = x, null, 
                BindingMode.TwoWay, true);

        private string _ipText;

        public string IpText
        {
            get => _ipText;
            set
            {
                SetAndRaise(IpTextProperty, ref _ipText, value);

                if (!IsValidIp(value))
                    ErrorText = "Invalid IP";
            }
        }

        private static bool IsValidIp(string ip)
        {
            return ip != null && !Regex.IsMatch(ip, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
        }


        public static readonly DirectProperty<MainPanel, string> PortTextProperty =
            AvaloniaProperty.RegisterDirect<MainPanel, string>(nameof(PortText),
                o => o.PortText,
                (o, x) => o.PortText = x, null,
                BindingMode.TwoWay, true);

        public int Port { get; private set; }
        private string _portText;

        public string PortText
        {
            get => _portText;
            set
            {
                SetAndRaise(PortTextProperty, ref _ipText, value);

                if (int.TryParse(value, out var port))
                    Port = port;
                else
                    ErrorText = "Invalid port";
            }
        }

        public static readonly DirectProperty<MainPanel, string> ErrorTextProperty =
            AvaloniaProperty.RegisterDirect<MainPanel, string>(nameof(ErrorText),
                o => o.ErrorText,
                (o, x) => o.ErrorText = x, null,
                BindingMode.TwoWay, true);

        private string _errorText;

        public string ErrorText
        {
            get => _errorText;
            set
            {
                SetAndRaise(IpTextProperty, ref _errorText, value);
            }
        }

        public static readonly DirectProperty<MainPanel, ICommand> CreateCommandProperty =
            AvaloniaProperty.RegisterDirect<MainPanel, ICommand>(nameof(CreateCommand), 
                o => o.CreateCommand, (o, x) => o.CreateCommand = x, enableDataValidation: true);

        private ICommand _createCommand;

        public ICommand CreateCommand
        {
            get => _createCommand;
            set => SetAndRaise(CreateCommandProperty, ref _createCommand, value);
        }

        public static readonly DirectProperty<MainPanel, ICommand> CreateAdvancedCommandProperty =
            AvaloniaProperty.RegisterDirect<MainPanel, ICommand>(nameof(CreateAdvancedCommand),
                o => o.CreateAdvancedCommand, (o, x) => o.CreateAdvancedCommand = x,
                null, BindingMode.TwoWay);

        private ICommand _createAdvancedCommand;

        public ICommand CreateAdvancedCommand
        {
            get => _createAdvancedCommand;
            set => SetAndRaise(CreateAdvancedCommandProperty, ref _createAdvancedCommand, value);
        }

        public static readonly DirectProperty<MainPanel, ICommand> JoinCommandProperty =
            AvaloniaProperty.RegisterDirect<MainPanel, ICommand>(nameof(JoinCommand),
                o => o.JoinCommand, (o, x) => o.JoinCommand = x,
                null, BindingMode.TwoWay);

        private ICommand _joinCommand;

        public ICommand JoinCommand
        {
            get => _joinCommand;
            set => SetAndRaise(CreateCommandProperty, ref _joinCommand, value);
        }
    }
}
