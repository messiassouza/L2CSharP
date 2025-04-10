using L2CSharP.DataBase; 
using L2CSharP.Network.Cliente.Interfaces;
 

namespace L2CSharP.Network.GameServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using L2CSharP.LoggerApi.Logger.Interfaces;
    using L2CSharP.Model;
    using L2CSharP.Network.Gameserver; // Certifique-se de que este namespace está referenciado corretamente

    public class ServerList : IServerList
    {
        private readonly AppDbContext _dbContext;
        private readonly IGameLogger _logger;

        // Implementação explícita da propriedade da interface
        SortedList<long, GameServersInfo> IServerList.GameServerList => GetConvertedGameServerList();

        // Propriedade interna para uso na classe (opcional, se necessário)
        private SortedList<long, GameServersInfo> InternalGameServerList { get; set; }

        public ServerList(AppDbContext dbContext, IGameLogger logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InternalGameServerList = new SortedList<long, GameServersInfo>();
        }

        public void Initialize()
        {
            try
            {
                _logger.Debug("Loading ServerList...");

                var servers = _dbContext.GameServers.ToList();

                foreach (var server in servers)
                {
                    InternalGameServerList.Add(server.Id, new GameServersInfo(
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

                _logger.Debug($"Loaded {InternalGameServerList.Count} game servers");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error initializing ServerList: {ex.Message}");
                throw;
            }
        }

        // Método público para uso interno (opcional)
        public GameServersInfo GetServerByIdInternal(long serverId)
        {
            if (InternalGameServerList.TryGetValue(serverId, out var serverInfo))
            {
                return serverInfo;
            }
            return null;
        }

        // Implementação explícita da interface
        GameServersInfo IServerList.GetServerById(long serverId)
        {
            var serverInfo = GetServerByIdInternal(serverId);
            if (serverInfo != null)
            {
                return ConvertToGameserverInfo(serverInfo);
            }
            return null;
        }

        // Método auxiliar para converter GameServersInfo para Gameserver.GameServersInfo
        private GameServersInfo ConvertToGameserverInfo(GameServersInfo serverInfo)
        {
            return new GameServersInfo(
                serverInfo.ServerID,
                serverInfo.InternalIP,
                serverInfo.ExternalIP,
                serverInfo.Port,
                serverInfo.AgeLimit,
                serverInfo.Pvp,
                serverInfo.CurPlayers,
                serverInfo.MaxPlayers,
                serverInfo.Online,
                serverInfo.ShowClock,
                serverInfo.Brackets,
                serverInfo.ServerHash);
        }

        // Método auxiliar para converter a lista interna para o tipo da interface
        private SortedList<long, GameServersInfo> GetConvertedGameServerList()
        {
            var convertedList = new SortedList<long, GameServersInfo>();
            foreach (var kvp in InternalGameServerList)
            {
                convertedList.Add(kvp.Key, ConvertToGameserverInfo(kvp.Value));
            }
            return convertedList;
        }
    }

    public interface IServerList
    {
        SortedList<long, GameServersInfo> GameServerList { get; }
        void Initialize();
        GameServersInfo GetServerById(long serverId);
    }
}
