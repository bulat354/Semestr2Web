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
        None = 0, 
        Player = 1, 
        Timer = 2,
        Character = 4,
        Action = 8,
        Person = 16,
        Message = 32,
        Voting = 64,

        Any = None | Player | Timer | Character | Action
    }

    [Flags]
    public enum MessageSecondGoal
    {
        None = 0, 
        Stop = 1,
        Add = 2,
        Remove = 4,
        Attach = 8,
        Detach = 16,
        ForTime = 32,

        Any = None | Stop | Add | Remove | Attach | Detach
    }
}
