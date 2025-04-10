using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IReceivePacket
    {
        int Offset { get; set; }
        GameClient Client { get; }
        byte[] Packet { get; }
        void Read();
        void Run();
    }
}
