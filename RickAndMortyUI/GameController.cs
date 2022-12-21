using RickAndMortyUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyUI
{
    public class GameController
    {
        public GameVM Game { get; set; }

        public IPlayerController[] playerControllers { get; set; }
    }
}
