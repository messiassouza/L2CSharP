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
 

namespace L2CSharP.Network.Gameserver.Packets.Receive
{
    using System;
    using System.Net;
    using System.Collections.Generic; 
    using Microsoft.Extensions.Logging;
    using global::L2CSharP.LoggerApi.Logger.Interfaces;
    using global::L2CSharP.Network.Cliente.Interfaces;
    using global::L2CSharP.Model;
    using global::L2CSharP.Network.GameServer;

    public class R_CloseConnection : ReceiveBasePacket
    {
        private static readonly IEqualityComparer<byte[]> ByteArrayComparer = new ByteArrayEqualityComparer();
        private readonly IGameLogger _logger;
        private readonly IServerList _serverList;

        public R_CloseConnection(IGameLogger logger, IServerList serverList, GameService client, byte[] packet)
            : base(client, packet, logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serverList = serverList ?? throw new ArgumentNullException(nameof(serverList));
        }

        public override void Read()
        {
            try
            {
                byte[] localIp = ReadBytes(4);
                byte[] wanIp = ReadBytes(4);
                short port = ReadShort();
                short serverId = ReadByte();

                LogConnectionDetails(localIp, wanIp, port, serverId);
                UpdateServerStatus(localIp, wanIp, port, serverId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error processing CloseConnection packet: {ex.Message}");
                throw;
            }
        }

        private void LogConnectionDetails(byte[] localIp, byte[] wanIp, short port, short serverId)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.Debug("GameServer Connection Close Request:");
                _logger.Debug($"- Internal IP: {new IPAddress(localIp)}");
                _logger.Debug($"- External IP: {new IPAddress(wanIp)}");
                _logger.Debug($"- Port: {port}");
                _logger.Debug($"- Server ID: {serverId}");
            }
        }

        private void UpdateServerStatus(byte[] localIp, byte[] wanIp, short port, short serverId)
        {
            var server = FindMatchingServer(localIp, wanIp, port, serverId);
            if (server != null)
            {
                server.CurPlayers = 0;
                server.Online = false;
                _logger.Debug($"GameServer {serverId} ({server.Name}) is now offline");
            }
        }

        private GameServersInfo FindMatchingServer(byte[] localIp, byte[] wanIp, short port, short serverId)
        {
            foreach (var server in _serverList.GameServerList.Values)
            {
                try
                {
                    if (IsServerMatch(server, localIp, wanIp, port, serverId))
                    {
                        return server;
                    }
                }
                catch (FormatException ex)
                {
                    _logger.Debug($"Invalid IP format in server {server.ServerID} configuration: {ex.Message}");
                }
            }
            return null;
        }

        private bool IsServerMatch(GameServersInfo server, byte[] localIp, byte[] wanIp, short port, short serverId)
        {
            var serverLocalIp = IPAddress.Parse(server.InternalIP).GetAddressBytes();
            var serverWanIp = IPAddress.Parse(server.ExternalIP).GetAddressBytes();

            return ByteArrayComparer.Equals(serverLocalIp, localIp) &&
                   ByteArrayComparer.Equals(serverWanIp, wanIp) &&
                   server.Port == port &&
                   server.ServerID == serverId;
        }

        public override void Run()
        {
            // Processamento principal já realizado no Read()
        }

        private class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
        {
            public bool Equals(byte[] x, byte[] y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;
                if (x.Length != y.Length) return false;

                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i]) return false;
                }
                return true;
            }

            public int GetHashCode(byte[] obj)
            {
                unchecked
                {
                    int hash = 17;
                    foreach (byte b in obj)
                    {
                        hash = hash * 31 + b;
                    }
                    return hash;
                }
            }
        }
    }
}
