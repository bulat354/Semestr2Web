using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace RickAndMortyUI.Controls
{
    public class CardControl : TemplatedControl
    {
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            selfPressed = new TaskCompletionSource();
            PointerPressed += (s, e) =>
            {
                if (IsClickable)
                    selfPressed.SetResult();
            };
        }

        public static readonly StyledProperty<IImage> SourceProperty =
            AvaloniaProperty.Register<CardControl, IImage>(nameof(Source));
        public IImage Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        public static readonly StyledProperty<bool> IsVisibleImageProperty =
            AvaloniaProperty.Register<CardControl, bool>(nameof(IsVisibleImage));
        public bool IsVisibleImage
        {
            get => GetValue(IsVisibleImageProperty);
            set => SetValue(IsVisibleImageProperty, value);
        }
        public static readonly StyledProperty<bool> IsClickableProperty =
            AvaloniaProperty.Register<CardControl, bool>(nameof(IsClickable));
        public bool IsClickable
        {
            get => GetValue<bool>(IsClickableProperty);
            set => SetValue<bool>(IsClickableProperty, value);
        }

        private TaskCompletionSource selfPressed;
        public Task IsPressed => selfPressed.Task;

        public void ResetPressed()
        {
            selfPressed.SetCanceled();
            selfPressed = new TaskCompletionSource();
        }
    }
}
