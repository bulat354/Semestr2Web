﻿using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Data;
using Avalonia;

namespace RickAndMortyUI.ViewModels
{
    public class CharactersPanelVM : ViewModelBase
    {
        public Grid MainGrid { get; set; }
        public GameVM GameVM { get; set; }

        public List<int> EmptyPlaces { get; set; } = new List<int>()
        {
            2, 1, 0
        };
        public int RowsCount = 1;

        public Control AddCharacter(CharacterVM vm)
        {
            var control = GetCharacterControl(vm);

            if (EmptyPlaces.Count == 0)
            {
                RowsCount++;
                EmptyPlaces.AddRange(new[] { RowsCount * 3 - 1, RowsCount * 3 - 2, RowsCount * 3 - 3 });
                OnWidthChanged();
            }

            var place = EmptyPlaces[EmptyPlaces.Count - 1];
            var row = place / 3;
            var col = place % 3;
            EmptyPlaces.RemoveAt(EmptyPlaces.Count - 1);

            MainGrid.AddChild(control, col, row);

            return control;
        }

        public void RemoveCharacter(Control control)
        {
            Dispatcher.UIThread.Post(() =>
            {
                MainGrid.Children.Remove(control);
                var row = control.GetValue(Grid.RowProperty);
                var col = control.GetValue(Grid.ColumnProperty);
                EmptyPlaces.Add(row * 3 + col);
                EmptyPlaces.Sort();
                EmptyPlaces.Reverse();
            });
        }

        public Control GetCharacterControl(CharacterVM vm)
        {
            /*
							<Border Grid.Column="0" Grid.Row="0" CornerRadius="7" ClipToBounds="True" Margin="20" VerticalAlignment="Top">
								<Image Source="/Assets/test.jpg"/>
							</Border>*/
            var task = Dispatcher.UIThread.InvokeAsync(() =>
            {
                var border = new Border();
                border.ClipToBounds = true;
                border.CornerRadius = new Avalonia.CornerRadius(7);
                border.Margin = new Avalonia.Thickness(20);
                border.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
                border.BoxShadow = new BoxShadows(BoxShadow.Parse("3 3 10 4 #40000000"));
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
                SetGridDefinition(RowsCount);
            });
        }

        public void SetGridDefinition(int rows)
        {
            var width = MainGrid.Bounds.Width / 3;
            var height = (width - 40) * (430.0 / 300.0) + 40;

            var str = string.Join(' ', Enumerable.Repeat(height.ToString().Replace(',', '.'), rows));
            MainGrid.RowDefinitions = RowDefinitions.Parse(str);
        }
    }
}
