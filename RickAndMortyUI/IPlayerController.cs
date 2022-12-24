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
        public int Id { get; }

        StringMessage? ProcessMessage(StringMessage message, bool isRequest = false);
    }
}
