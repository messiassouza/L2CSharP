using L2CSharP.Model;
using L2CSharP.Network.Gameserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IServerListService
    {
        Task InitializeAsync();
        Task<GameServersInfo> GetServerInfoAsync(int serverId);
        Task<IEnumerable<GameServersInfo>> GetAllServersAsync();
        Task UpdateServerStatusAsync(int serverId, bool isOnline, int currentPlayers);
    }
}
