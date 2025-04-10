using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using L2CSharP.Network.Gameserver;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IGameServerProcessor
    {
        Task ProcessGameServerAsync(L2CSharP.Network.Gameserver.GameService gameServer);
        Task HandleServerDisconnectionAsync(L2CSharP.Network.Gameserver.GameService gameServer);
        Task HandleServerMessageAsync(L2CSharP.Network.Gameserver.GameService gameServer, byte[] message);
    }
}
