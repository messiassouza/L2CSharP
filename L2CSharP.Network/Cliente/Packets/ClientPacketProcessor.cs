using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Cliente.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Packets
{
    public class PacketProcessor : IPacketProcessor
    {
        private readonly IGameLogger _logger;
        private readonly SortedList<short, PacketType> _packetList;

        public PacketProcessor(IGameLogger logger)
        {
            _logger = logger;
            _packetList = new SortedList<short, PacketType>();
        }

        public void Initialize()
        {
            _logger.Debug("Registering packets." );
            // Pode adicionar registros padrão aqui se necessário
        }

        public void RegisterPacket(PacketType packet)
        {
            _logger.Debug($"New packet registered: Opcode: {packet.Opcode:x4} Name: {packet.Name}");

            if (_packetList.ContainsKey(packet.Opcode))
            {
                throw new ArgumentException($"Packet with opcode {packet.Opcode} already registered");
            }

            _packetList.Add(packet.Opcode, packet);
        }

        public Type ProcessPacket(byte[] packet)
        {
            if (packet == null || packet.Length == 0)
            {
                _logger.Debug("Empty packet received" );
                return null;
            }

            short opcode = packet[0];
            if (_packetList.TryGetValue(opcode, out var packetType))
            {
                _logger.Debug($"Incoming packet {packetType.Name}");
                return packetType.Packet;
            }

            _logger.Debug($"Unknown Incoming packet {BitConverter.ToString(packet)}");
            return null;
        }
    }

    // Implementação concreta de IPacketType
    public class PacketType : IPacketType
    {
        public short Opcode { get; }
        public Type Packet { get; }

        public string Name { get; } = string.Empty;

        public PacketType(short opcode, Type packetType)
        {
            Opcode = opcode;
            Packet = packetType ?? throw new ArgumentNullException(nameof(packetType));
        }
    }
}
