using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Test
{
    [Flags]
    public enum MessageFirstGoal
    {
        None = 1, 
        Player = 2, 
        Timer = 4,
        Character = 8,
        Action = 16,

        Any = None | Player | Timer | Character | Action
    }

    [Flags]
    public enum MessageSecondGoal
    {
        None = 1, 
        Stop = 2,
        Add = 4,
        Remove = 8,
        Attach = 16,
        Detach = 32,

        Any = None | Stop | Add | Remove | Attach | Detach
    }
}
