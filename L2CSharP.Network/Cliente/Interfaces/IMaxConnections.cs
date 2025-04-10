using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IMaxConnections
    {
        bool AcceptConnection(string ip);
        void Disconnect(string ip);
    }
}
