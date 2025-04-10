// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
// 
// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
// 
using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Cliente.Interfaces;
using L2CSharP.Network.Cliente.Packets;
using L2CSharP.Network.Gameserver.Packets.Receive;
using L2CSharP.Network.GameServer.Packets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text; 


namespace L2CSharP.Network.Gameserver.Packets
{
    public class GamePacketProcessor : IGamePacketProcessor
    {
        private readonly IGameLogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<short, Type> _packetRegistry = new();

        public GamePacketProcessor(
            IGameLogger logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void Initialize()
        {
            try
            {
                // Exemplo de registro de pacotes
                RegisterPacket(new PacketType(0x0001, typeof(R_ServerInfo)));
                // Adicione outros pacotes aqui

                _logger.LogInformation($"Packet processor initialized with {_packetRegistry.Count} registered packets");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize packet processor");
                throw;
            }
        }

        public void RegisterPacket(IPacketType packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (!typeof(GamePacketBase).IsAssignableFrom(packet.Packet))
            {
                throw new ArgumentException($"Packet type {packet.Packet.Name} must inherit from GamePacketBase");
            }

            _packetRegistry[packet.Opcode] = packet.Packet;
            _logger.LogDebug($"Registered packet: 0x{packet.Opcode:X4} - {packet.Packet.Name}");
        }

        public Type GetPacketType(short opcode)
        {
            if (_packetRegistry.TryGetValue(opcode, out var packetType))
            {
                return packetType;
            }
            return null;
        }

        public Type ProcessPacket(byte[] packetData)
        {
            if (packetData == null || packetData.Length < 2)
            {
                _logger.LogWarning("Invalid packet data received");
                return null;
            }

            short opcode = BitConverter.ToInt16(packetData, 0);
            return GetPacketType(opcode);
        }

        public GamePacketBase CreatePacket(GameService client, byte[] data)
        {
            try
            {
                short opcode = BitConverter.ToInt16(data, 0);
                var packetType = GetPacketType(opcode);

                if (packetType == null)
                {
                    _logger.LogWarning($"Unknown packet opcode: 0x{opcode:X4}");
                    return null;
                }

                return (GamePacketBase)ActivatorUtilities.CreateInstance(
                    _serviceProvider,
                    packetType,
                    client,
                    data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create packet instance");
                return null;
            }
        }
    }



}
