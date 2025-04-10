using L2CSharP.Network.Cliente.Packets;
using L2CSharP.Network.GameServer.Packets;
using L2CSharP.Network.Gameserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{ 
    public interface IGamePacketProcessor
    {
        void Initialize();
        void RegisterPacket(IPacketType packet);
        Type GetPacketType(short opcode);
        Type ProcessPacket(byte[] packetData);
        GamePacketBase CreatePacket(GameService client, byte[] data);
    }
}
