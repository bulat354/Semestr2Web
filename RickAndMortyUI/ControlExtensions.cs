using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyUI
{
    public static class ControlExtensions
    {
        public static void AddChild(this Grid grid, Control control, int column = 0, int row = 0, int columnSpan = 1, int rowSpan = 1)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (column > 0)
                    control.SetValue(Grid.ColumnProperty, column);
                if (row > 0)
                    control.SetValue(Grid.RowProperty, row);
                if (columnSpan > 1)
                    control.SetValue(Grid.ColumnSpanProperty, columnSpan);
                if (rowSpan > 1)
                    control.SetValue(Grid.RowSpanProperty, rowSpan);

                grid.Children.Add(control);
            });
        }

        public static void MakeClickable(this Control control)
        {
            Dispatcher.UIThread.Post(() =>
            {
                control.Classes.Add("clickable");
            });
        }

        public static void MakeUnclickable(this Control control)
        {
            Dispatcher.UIThread.Post(() =>
            {
                control.Classes.Remove("clickable");
            });
        }
    }
}
