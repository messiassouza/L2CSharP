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

using System.Net;
using System.Reflection.Emit;
using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Cliente.Interfaces;
using L2CSharP.Network.GameServer;
using L2CSharP.Network.GameServer.Packets;
using Microsoft.Extensions.Logging;
using NLog;

namespace L2CSharP.Network.Gameserver.Packets.Receive
{

    public class R_ServerInfo : GamePacketBase
    {
        private readonly IAccountService _accountService;
        private readonly IServerList _serverList;

        private byte[] _localIp;
        private byte[] _wanIp;
        private short _port;
        private short _ageLimit;
        private bool _pvp;
        private short _currentPlayers;
        private short _maxPlayers;
        private bool _online;
        private bool _showClock;
        private bool _brackets;
        private short _serverId;
        private string _serverHash;

        public R_ServerInfo(
            GameService client,
            byte[] packet,
            IGameLogger logger,
            IAccountService accountService,
            IServerList serverList)
            : base(client, packet, logger)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _serverList = serverList ?? throw new ArgumentNullException(nameof(serverList));

            Opcode = 0x0001;
            Name = "Server Information";
        }

        public override void Read()
        {
            try
            {
                _localIp = ReadBytes(4);
                _wanIp = ReadBytes(4);
                _port = ReadShort();
                _ageLimit = ReadByte();
                _pvp = ReadBool();
                _currentPlayers = ReadShort();
                _maxPlayers = ReadShort();
                _online = ReadBool();
                _showClock = ReadBool();
                _brackets = ReadBool();
                _serverId = ReadByte();
                _serverHash = ReadString();

                LogServerDetails();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to read R_ServerInfo packet");
                throw;
            }
        }

        public override void Run()
        {
            try
            {
                UpdateServerInformation();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to update server information");
                throw;
            }
        }

        private void LogServerDetails()
        {
            if (Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
            {
                Logger.LogDebug("Server Info Packet Details:\n" +
                    $"Server ID: {_serverId}\n" +
                    $"Internal IP: {new IPAddress(_localIp)}\n" +
                    $"External IP: {new IPAddress(_wanIp)}\n" +
                    $"Port: {_port}\n" +
                    $"Status: {(_online ? "Online" : "Offline")}\n" +
                    $"Players: {_currentPlayers}/{_maxPlayers}\n" +
                    $"Age Limit: {_ageLimit}+");
            }
        }

        private void UpdateServerInformation()
        {
            foreach (var server in _serverList.GameServerList.Values)
            {
                if (server.ServerHash == _serverHash)
                {
                    server.InternalIP = new IPAddress(_localIp).ToString();
                    server.ExternalIP = new IPAddress(_wanIp).ToString();
                    server.Port = _port;
                    server.AgeLimit = _ageLimit;
                    server.Pvp = _pvp;
                    server.CurPlayers = _currentPlayers;
                    server.MaxPlayers = _maxPlayers;
                    server.Online = _online;
                    server.ShowClock = _showClock;
                    server.Brackets = _brackets;

                    Logger.LogInformation($"Updated server {_serverId} in server list");
                    break;
                }
            }
        }
    }

}
