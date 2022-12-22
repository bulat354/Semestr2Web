using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Threading;

namespace RickAndMortyUI.ViewModels
{
    public class WaitVM : GridVM
    {
        public PlayerIconVM[] IconVMs { get; set; } =
            new PlayerIconVM[]
            {
                new PlayerIconVM(),
                new PlayerIconVM(),
                new PlayerIconVM(),
                new PlayerIconVM(),
                new PlayerIconVM()
            };
        
        private string _waitText;
        public string WaitText
        {
            get => _waitText;
            set => this.RaiseAndSetIfChanged(ref _waitText, value);
        }

        private string _counter;
        public string Counter
        {
            get => _counter;
            set => this.RaiseAndSetIfChanged(ref _counter, value);
        }

        public bool CountingEnded { get; private set; }

        private bool ableToSetText;

        public void Show(int index)
        {
            IconVMs[index].Show();
        }

        public void Hide(int index)
        {
            IconVMs[index].Hide();
        }

        public void Bind(int index, IImage image)
        {
            IconVMs[index].Bind(image);
        }

        public async Task StartCounting(int count, CancellationTokenSource src)
        {
            var i = count;
            Counter = i.ToString();

            while (i > 0)
            {
                try
                {
                    await Task.Delay(1000, src.Token);
                }
                catch
                {
                    break;
                }

                i--;
                Counter = i.ToString();
            }

            Counter = "0";
            src.Cancel();
        }

        public async Task StartAnimation(CancellationToken token)
        {
            var i = 0;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(800, token);
                }
                catch
                {
                    break;
                }

                i = (i + 1) % 4;
                if (ableToSetText)
                    WaitText = "Ожидание игроков" + new string('.', i);
            }

            CountingEnded = true;
            WaitText = "Игра начинается!";
        }

        public async Task ShowText(string text, int time)
        {
            ableToSetText = false;
            WaitText = text;
            await Task.Delay(time);
            ableToSetText = true;
        }

        public int GetEmptyPlayer()
        {
            if (IconVMs[2].Id < 0)
                return 2;

            for (int i = 0; i < IconVMs.Length; i++)
            {
                if (IconVMs[i].Id < 0)
                    return i;
            }

            return -1;
        }

        public override void Reset()
        {
            base.Reset();

            foreach (var item in IconVMs)
                item.Hide();

            WaitText = "Ожидание игроков";
            Counter = string.Empty;
            CountingEnded = false;
        }
    }
}
