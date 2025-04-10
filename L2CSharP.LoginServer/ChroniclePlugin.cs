using L2CSharP.Network.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.LoginServer
{
    public abstract class ChroniclePlugin
    {
        public abstract void OnClientConnect(GameClient client);
    }
}
