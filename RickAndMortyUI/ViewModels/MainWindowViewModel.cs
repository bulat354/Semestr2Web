using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
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
        public MenuVM menuGridVM { get; set; } = new MenuVM();
        public WaitVM waitGridVM { get; set; } = new WaitVM();
        public GameVM gameGridVM { get; set; } = new GameVM();
    }
}
