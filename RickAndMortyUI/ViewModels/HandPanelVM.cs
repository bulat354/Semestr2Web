using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyUI.ViewModels
{
    public class HandPanelVM : ViewModelBase
    {
        public Grid MainGrid { get; set; }
        public GameVM GameVM { get; set; }

        public int ColumnsCount { get; set; }

        public Control AddAction(CharacterVM vm)
        {
            ColumnsCount++;

            var control = GetActionControl(vm);
            MainGrid.AddChild(control, ColumnsCount, 0, 2);
            OnWidthChanged();

            return control;
        }

        public void RemoveAction(Control control)
        {
            ColumnsCount--;

            Dispatcher.UIThread.Post(() =>
            {
                MainGrid.Children.Remove(control);

                SetGridDefinition(ColumnsCount);
            });
        }

        public Control GetActionControl(CharacterVM vm)
        {
            /*
						<Border Grid.Column="1" CornerRadius="2" Grid.ColumnSpan="10" HorizontalAlignment="Left" ClipToBounds="True">
							<Image Source="/Assets/test1.jpg" VerticalAlignment="Top"/>
						</Border>*/
            var task = Dispatcher.UIThread.InvokeAsync(() =>
            {
                var border = new Border();
                border.ClipToBounds = true;
                border.CornerRadius = new Avalonia.CornerRadius(2);
                border.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                border.BorderBrush = new SolidColorBrush(Color.Parse("#000000"), 0.2);
                border.BorderThickness = new Avalonia.Thickness(1, 0, 0, 0);
                border.DataContext = vm;

                var image = new Image();
                image.Bind(Image.SourceProperty, new Binding("Source"));
                image.PointerEnter += GameVM.OnPointerEnter;
                image.PointerLeave += GameVM.OnPointerLeave;

                border.Child = image;
                return border;
            });
            task.Wait();
            return task.Result;
        }

        public void OnWidthChanged()
        {
            Dispatcher.UIThread.Post(() =>
            {
                SetGridDefinition(ColumnsCount);
            });
        }

        public void SetGridDefinition(int columns)
        {
            MainGrid.ColumnDefinitions = ColumnDefinitions.Parse($"0.5* {string.Join(' ', Enumerable.Repeat("*", columns))} 0.5*");

            var i = 1;
            foreach (var control in MainGrid.Children)
            {
                control.SetValue(Grid.ColumnProperty, i);
                i++;
            }
        }
    }
}
