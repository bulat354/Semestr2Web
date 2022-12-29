using Avalonia.Controls;
using Avalonia.Media;
using RickAndMortyUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyUI
{
    public class VisualCard
    {
        public Control Control { get; set; }
        public int Id { get; set; } = -1;
        public int PlayerId { get; set; } = -1;
        public CharacterVM? Vm { get; set; }

        public void MakeClickable()
        {
            if (Control is Image)
                Control.Parent.MakeClickable();
            else
                Control.MakeClickable();
        }

        public void MakeUnclickable()
        {
            if (Control is Image)
                Control.Parent.MakeUnclickable();
            else
                Control.MakeUnclickable();
        }
    }
}
