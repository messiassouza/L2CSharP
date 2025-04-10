using L2CSharP.Network.Cliente.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IPacketProcessor
    {
        void RegisterPacket(PacketType packet);
        Type ProcessPacket(byte[] packet);
        void Initialize();
    }
}
