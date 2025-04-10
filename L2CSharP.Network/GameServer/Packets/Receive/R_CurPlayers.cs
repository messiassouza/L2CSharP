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
using L2CSharP.Network.GameServer.Packets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;


namespace L2CSharP.Network.Gameserver.Packets.Receive
{


    using System;
    using System.Net; 
    using Microsoft.Extensions.Logging;

    namespace L2CSharP.Network.Gameserver.Packets.Receive
    {
        public class R_CurPlayers : GamePacketBase
        {
            private int _currentPlayerCount;
            private DateTime _packetReceivedTime;

            public R_CurPlayers(GameService client, byte[] packet, IGameLogger logger)
                : base(client, packet, logger)
            {
                Opcode = 0x0000; // Opcode para Current Players
                Name = "Current Players";
                _packetReceivedTime = DateTime.UtcNow;
            }

            public override void Read()
            {
                try
                {
                    _currentPlayerCount = ReadInt32();

                    // Validação básica dos dados
                    if (_currentPlayerCount < 0)
                    {
                        Logger.LogWarning($"Invalid player count received: {_currentPlayerCount}");
                        _currentPlayerCount = 0;
                    }

                    Logger.LogDebug($"Current players packet received - Count: {_currentPlayerCount}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to read current players packet");
                    throw;
                }
            }

            public override void Run()
            {
                try
                {
                    // Atualiza a contagem de jogadores no servidor
                    Client.CurrentPlayers = _currentPlayerCount;

                    // Log informativo
                    Logger.LogInformation($"[{_packetReceivedTime:HH:mm:ss}] " +
                                        $"Server: {Client.RemoteEndPoint} - " +
                                        $"Players Online: {_currentPlayerCount}");

                    // Verificação de capacidade
                    CheckServerCapacity();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to process current players update");
                    throw;
                }
            }

            private void CheckServerCapacity()
            {
                //if (Client.MaxPlayers > 0 && _currentPlayerCount > Client.MaxPlayers)
                //{
                //    Logger.LogWarning($"Server over capacity! Current: {_currentPlayerCount} / Max: {Client.MaxPlayers}");
                //    Client.NotifyOvercapacity();
                //}
            }
        }
    }


}
