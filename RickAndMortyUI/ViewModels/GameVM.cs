using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia.Media;
using Avalonia.Input;
using Avalonia.Controls;

namespace RickAndMortyUI.ViewModels
{
    public class GameVM : GridVM
    {
        public CharactersPanelVM CharactersPanel { get; } = new CharactersPanelVM();
        public HandPanelVM HandPanel { get; } = new HandPanelVM();

        public CharacterVM[] PlayerCharaterVms { get; } = new CharacterVM[]
        {
            new CharacterVM(),
            new CharacterVM(),
            new CharacterVM(),
            new CharacterVM(),
            new CharacterVM()
        };

        public CharacterVM PersonVM { get; } = new CharacterVM();

        public PlayerIconVM[] PlayerIconVMs { get; set; }

        private string _gameText;
        public string GameText
        {
            get => _gameText;
            set => this.RaiseAndSetIfChanged(ref _gameText, value);
        }
        private bool _overlayVisible;
        public bool OverlayVisible
        {
            get => _overlayVisible;
            set => this.RaiseAndSetIfChanged(ref _overlayVisible, value);
        }
        private IImage _overlaySource;
        public IImage OverlaySource
        {
            get => _overlaySource;
            set => this.RaiseAndSetIfChanged(ref _overlaySource, value);
        }

        public override void Reset()
        {
            CharactersPanel.GameVM = this;
            HandPanel.GameVM = this;
        }

        public void OnPointerEnter(object? sender, PointerEventArgs args)
        {
            if (sender is Control control)
            {
                if (control.DataContext is CharacterVM vm)
                {
                    if (vm != null && vm.IsVisible)
                    {
                        OverlaySource = vm.Source;
                        OverlayVisible = true;
                    }
                }
            }
        }

        public void OnPointerLeave(object? sender, PointerEventArgs args)
        {
            if (sender is Control control)
            {
                if (control.DataContext is CharacterVM vm)
                {
                    if (vm != null && vm.IsVisible)
                    {
                        OverlayVisible = false;
                    }
                }
            }
        }

        public async Task ShowText(string text, int time = -1)
        {
            if (time < 0)
                GameText = text;
            else
            {
                var oldText = GameText;
                GameText = text;
                await Task.Delay(time);
                if (GameText == text)
                    GameText = oldText;
            }
        }
    }
}
