using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using RickAndMortyLibrary.Common;

namespace RickAndMortyUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _backImage = "C:/git/background.png";
        public string BackgroundImagePath
        {
            get { return _backImage; }
            set { _backImage = value; this.RaisePropertyChanged(nameof(BackgroundImagePath)); }
        }

        public void ButtonCreateClick()
        {

        }

        public void ButtonJoinClick()
        {

        }
    }
}
