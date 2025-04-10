using L2CSharP.DataBase;
using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Model;
using L2CSharP.Network.Cliente.Interfaces;
using L2CSharP.Network.Gameserver;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.LoginServer
{
    public class ServerListService : IServerListService
    {
        private readonly AppDbContext _dbContext;
        private readonly IGameLogger _logger;
        private readonly SortedList<int, GameServersInfo> _gameServers;

        public ServerListService(AppDbContext dbContext, IGameLogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _gameServers = new SortedList<int, GameServersInfo>();
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Loading ServerList...", LogLevel.Information);

            var servers = await _dbContext.GameServers.ToListAsync();
            foreach (var server in servers)
            {
                _gameServers.Add((int)server.Id, new GameServersInfo(
                    (short)server.Id,
                    "0.0.0.0", // InternalIP padrão
                    "0.0.0.0", // ExternalIP padrão
                    (short)server.Port,
                    0,         // AgeLimit padrão
                    false,     // Pvp padrão
                    0,         // CurPlayers padrão
                    1000,      // MaxPlayers padrão
                    false,     // Online padrão
                    server.IsTestServer == 1, // ShowClock
                    server.Brackets == 1,     // Brackets
                    server.ServerHash));
            }

            _logger.LogInformation($"Loaded {_gameServers.Count} game servers");
        }

        public async Task<GameServersInfo> GetServerInfoAsync(int serverId)
        {
            if (_gameServers.TryGetValue(serverId, out var serverInfo))
            {
                return serverInfo;
            }
            return null;
        }

        public async Task<IEnumerable<GameServersInfo>> GetAllServersAsync()
        {
            return _gameServers.Values;
        }

        public async Task UpdateServerStatusAsync(int serverId, bool isOnline, int currentPlayers)
        {
            if (_gameServers.TryGetValue(serverId, out var serverInfo))
            {
                serverInfo.IsOnline = isOnline;
                serverInfo.CurrentPlayers = currentPlayers;

                // Atualiza também no banco de dados
                var server = await _dbContext.GameServers.FindAsync(serverId);
                if (server != null)
                {
                    server.IsOnline = isOnline;
                    server.CurrentPlayers = currentPlayers;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
