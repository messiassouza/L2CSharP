using L2CSharP.Model;
using L2CSharP.Network.Gameserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IServerManager
    {
        GameServersInfo FindServer(byte[] localIp, byte[] wanIp, short port, short serverId);
        void NotifyServerStatusChange(short serverId, bool isOnline);
    }
}
