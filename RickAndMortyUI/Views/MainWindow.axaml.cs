using Avalonia.Controls;
using Avalonia;
using System.IO;
using System;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Styling;

namespace RickAndMortyUI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var path = Path.Combine(Directory.GetCurrentDirectory().Split("RickAndMortyUI\\", StringSplitOptions.RemoveEmptyEntries)[0], "images\\");
            this.Background = new ImageBrush(new Bitmap(path + "background.png")) { Stretch = Stretch.UniformToFill };

            var logo = this.logo;
            logo.Source = new Bitmap(path + "logo.png");
        }
    }
}
