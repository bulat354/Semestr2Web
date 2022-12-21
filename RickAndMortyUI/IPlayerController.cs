using RickAndMortyLibrary.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyUI
{
    public interface IPlayerController
    {
        Task<StringMessage> ProcessMessage(StringMessage message);
    }
}
