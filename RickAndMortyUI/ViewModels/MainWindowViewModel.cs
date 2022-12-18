using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ReactiveUI;
using RickAndMortyLibrary.Common;
using RickAndMortyUI.Views;

namespace RickAndMortyUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindow Window { get; set; }

        private double _visibility1 = 0;
        private double _visibility2 = 0;

        private int _dotsCount = 0;
        private int _countdown = 30;

        #region Properties
        public double MainVisibility
        {
            get => this._visibility1;
            set => this.RaiseAndSetIfChanged(ref _visibility1, value);
        }
        public double WaitVisibility
        {
            get => this._visibility2;
            set => this.RaiseAndSetIfChanged(ref _visibility2, value);
        }

        public string WaitText
        {
            get => $"ќжидание игроков{new string('.', _dotsCount)}";
            set => this.RaiseAndSetIfChanged(ref _dotsCount, (_dotsCount + 1) % 4);
        }

        public int WaitTimer
        {
            get => _countdown;
            set => this.RaiseAndSetIfChanged(ref _countdown, value);
        }
        #endregion

        public async void StartWaitingTextAnim(int durMilliSec, CancellationTokenSource source)
        {
            while (!source.IsCancellationRequested)
            {
                await Task.Delay(durMilliSec, source.Token);
                WaitText = "";
                WaitTimer--;

                if (WaitTimer == 0)
                {
                    source.Cancel();
                    return;
                }
            }
        }

        public void ToWaitScreen()
        {
            MainVisibility = 0;
            WaitVisibility = 1;
        }
    }
}
