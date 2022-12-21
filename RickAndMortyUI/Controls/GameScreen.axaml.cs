using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RickAndMortyUI.Controls
{
    public class GameScreen : TemplatedControl
    {
        private WrapPanel CharactersPanel;
        private StackPanel HandPanel;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            HandPanel = e.NameScope.Find<StackPanel>("HandPanel");
        }

        public static readonly StyledProperty<IImage> OverlayImageProperty =
            AvaloniaProperty.Register<GameScreen, IImage>(nameof(OverlayImage));

        public IImage OverlayImage
        {
            get => GetValue(OverlayImageProperty);
            set => SetValue(OverlayImageProperty, value);
        }
        public static readonly StyledProperty<bool> OverlayVisibleProperty =
            AvaloniaProperty.Register<GameScreen, bool>(nameof(OverlayVisible));

        public bool OverlayVisible
        {
            get => GetValue(OverlayVisibleProperty);
            set => SetValue(OverlayVisibleProperty, value);
        }

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<GameScreen, string>(nameof(Text));

        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ImageInfo[] PlayerCharacters { get; set; } = new ImageInfo[5]
        {
            new ImageInfo(),
            new ImageInfo(),
            new ImageInfo(),
            new ImageInfo(),
            new ImageInfo()
        };

        public ImageInfo Person { get; set; } = new ImageInfo();

        int lastIndex = 0;
        public Dictionary<int, Control> HandControls = new Dictionary<int, Control>();

        public int AddToHand(IImage image)
        {
            var control = CreateActionControl(image);

            HandPanel.Children.Add(control);
            HandControls.Add(lastIndex, control);
            lastIndex++;

            UpdateHand();

            return lastIndex - 1;
        }

        public void RemoveFromHand(int index)
        {
            if (HandControls.TryGetValue(index, out var control))
            {
                HandPanel.Children.Remove(control);
                HandControls.Remove(lastIndex);
                
                UpdateHand();
            }
        }

        public Control CreateActionControl(IImage source)
        {
            var border1 = new Border();
            //border1.Classes.Add("overlay");
            border1.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            border1.PointerEnter += (s, e) =>
            {
                OverlayImage = source;
                OverlayVisible = true;
            };
            border1.PointerLeave += (s, e) =>
            {
                OverlayVisible = false;
            };
            
            var border2 = new Border();
            //border2.Classes.Add("card");
            //border2.Classes.Add("moved");

            var image = new Image();
            //image.Classes.Add("char");
            image.Width = 150;
            image.Source = source;

            border2.Child = image;
            border1.Child = border2;

            return border1;
        }

        public Control CreateCharacterControl(ImageInfo info)
        {
            throw new NotImplementedException();
        }

        public void UpdateHand()
        {
            var count = HandControls.Count;
            if (count < 2) 
                return;

            var actWidth = 150 * count;
            var delta = HandPanel.Width - actWidth;
            var transform = delta / (count - 1);

            var i = 1;
            foreach (var control in HandControls.Skip(1))
            {
                var tr = transform * i;
                control.Value.SetValue(RenderTransformProperty,
                    new TranslateTransform() { X = tr });
                i++;
            }
        }

        public ImageInfo[] Players { get; set; } = new ImageInfo[]
        {
            new ImageInfo(),
            new ImageInfo(),
            new ImageInfo(),
            new ImageInfo(),
            new ImageInfo()
        };

        public ImageInfo GetImageInfo(IImage image = null, bool isVisible = false)
        {
            return new ImageInfo()
            {
                Image = image,
                IsVisible = isVisible
            };
        }
    }

    public class ImageInfo
    {
        public IImage Image { get; set; }
        public bool IsVisible { get; set; }
    }
}
