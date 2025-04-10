using L2CSharP.Network.Crypt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente.Interfaces
{
    public interface IGameClientProcessor
    {
        // Na interface IGameClientProcessor:
        ScrambledKeyPair GenerateScrambledPair();
        Task ProcessClientAsync(Socket client);
        int ConnectedClientCount { get; }
        IReadOnlyCollection<GameClient> Clients { get; }
        byte[] GenerateBlowfishKey(); 
    }
}
