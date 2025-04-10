using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IPacketType
    {
        string Name { get; }
        short Opcode { get; }
        Type Packet { get; }
    }
}
