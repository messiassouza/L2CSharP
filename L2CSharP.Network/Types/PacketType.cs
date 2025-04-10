using L2CSharP.Network.Cliente.Interfaces;
using L2CSharP.Network.GameServer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Types
{

    public class PacketType : IPacketType
    {
        public string Name { get; }
        public short Opcode { get; }
        public Type Packet { get; }

        public PacketType(string name, short opcode, Type packetType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Opcode = opcode;
            Packet = packetType ?? throw new ArgumentNullException(nameof(packetType));

            ValidatePacketType();
        }

        private void ValidatePacketType()
        {
            // Verifica se o tipo é uma classe válida de pacote
            if (!typeof(GamePacketBase).IsAssignableFrom(Packet))
            {
                throw new ArgumentException($"Packet type {Packet.Name} must inherit from GamePacketBase");
            }
        }
    }


}
