using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network
{
    public class NetworkSettings
    {
        // Configurações para clientes
        public string ClientListenAddr { get; set; } = "0.0.0.0";
        public int ClientListenPort { get; set; } = 7777;
        public int ClientBacklog { get; set; } = 100;

        // Configurações para game servers
        public string GameServerListenAddr { get; set; } = "0.0.0.0";
        public int GameServerListenPort { get; set; } = 7778;
        public int GameServerBacklog { get; set; } = 50;

        // Configurações de segurança e desempenho
        public int BlowfishKeyCount { get; set; } = 50;
        public int ScrambledPairCount { get; set; } = 10;
        public int FloodProtectionLimit { get; set; } = 5;
        public int FloodProtectionInterval { get; set; } = 1000;
        public int ConnectionDelay { get; set; } = 50;
        public int MaxConnectionsPerIp { get; set; } = 10;
        public int AcceptLoopDelayMs { get; set; } = 50;
        public int ErrorRetryDelayMs { get; set; } = 1000;
        public int SocketBufferSize { get; set; } = 8192;
        public int SocketTimeoutMs { get; set; } = 30000;
    }
}
