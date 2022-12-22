using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace RickAndMortyUI.ViewModels
{
    public class CharacterVM : ViewModelBase
    {
        public int CardId { get; set; }

        private bool _isVisible = false;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { this.RaiseAndSetIfChanged(ref _isVisible, value); }
        }

        private IImage _source;
        public IImage Source
        {
            get { return _source; }
            set { this.RaiseAndSetIfChanged(ref _source, value); }
        }

        public void Hide()
        {
            IsVisible = false;
        }

        public void Show()
        {
            IsVisible = true;
        }

        public void Bind(IImage image)
        {
            Source = image;
        }
    }
}
