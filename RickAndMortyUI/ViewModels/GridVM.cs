using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RickAndMortyUI.Views;
using ReactiveUI;

namespace RickAndMortyUI.ViewModels
{
    public class GridVM : ViewModelBase
    {
        private double _opacity = 0;
        public double Opacity
        {
            get => _opacity;
            set => this.RaiseAndSetIfChanged(ref _opacity, value);
        }

        private bool _visible = false;
        public bool Visible
        {
            get => _visible;
            set => this.RaiseAndSetIfChanged(ref _visible, value);
        }

        public async void Appear()
        {
            Visible = true;
            Opacity = 0;
            await Task.Delay(500);
            Opacity = 1;
        }

        public async void Disappear()
        {
            Opacity = 0;
            await Task.Delay(500);
            Visible = false;
        }
    }
}
