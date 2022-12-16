using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMortyLibrary.Messages
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class PacketTypeAttribute : Attribute
    {
        public byte PacketType { get; }

        public PacketTypeAttribute(byte packetType)
        {
            PacketType = packetType;
        }
    }
}
