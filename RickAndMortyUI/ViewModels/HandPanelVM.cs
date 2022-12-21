using Avalonia.Controls;
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

        public int ColumnsCount { get; set; }

        public async Task<Control> AddAction(IImage source)
        {
            ColumnsCount++;

            var control = await GetActionControl(source);
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

        public async Task<Control> GetActionControl(IImage source)
        {
            /*
						<Border Grid.Column="1" CornerRadius="2" Grid.ColumnSpan="10" HorizontalAlignment="Left" ClipToBounds="True">
							<Image Source="/Assets/test1.jpg" VerticalAlignment="Top"/>
						</Border>*/
            return await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var border = new Border();
                border.ClipToBounds = true;
                border.CornerRadius = new Avalonia.CornerRadius(2);
                border.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left;
                border.BorderBrush = new SolidColorBrush(Color.Parse("#000000"), 0.2);
                border.BorderThickness = new Avalonia.Thickness(1, 0, 0, 0);

                var image = new Image();
                image.Source = source;

                border.Child = image;
                return border;
            });
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
