using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Test
{
    [Flags]
    public enum CharacterTag
    {
        None = 0,
        Immutable = 1,
        CanKill = 2

    }
}
