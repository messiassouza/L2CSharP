using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Cliente.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente
{
    public class MaxConnections : IMaxConnections  // Explicitly implement the interface
    {
        private readonly SortedList<string, int> _connections;  // Changed from static to instance
        private readonly int _maxConnections = 100;
        private readonly IGameLogger _logger;

        public MaxConnections(IGameLogger logger)
        {
            _connections = new SortedList<string, int>();
            _logger = logger;
            _logger.Debug("Initialized MaxConnections");
        }

        private void AddConnection(string IP)  // Changed from static to instance
        {
            string TempIP = IP.Split(':')[0];  // Removed redundant ToString()

            if (!_connections.ContainsKey(TempIP))
                _connections.Add(TempIP, 0);
            else
                _connections[TempIP]++;
        }

        public void Disconnect(string IP)  // Changed from static to instance
        {
            string TempIP = IP.Split(':')[0];  // Removed redundant ToString()
            if (_connections.ContainsKey(TempIP))
                _connections[TempIP]--;

            if (_connections.ContainsKey(TempIP) && _connections[TempIP] <= 0)
            {
                _connections.Remove(TempIP);
            }
        }

        public bool AcceptConnection(string IP)  // Changed from static to instance
        {
            AddConnection(IP);
            string TempIP = IP.Split(':')[0];
            return _connections[TempIP] <= _maxConnections;
        }
    }

}
