// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
// 
// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using L2CSharP.Network.Cliente.Interfaces;
using L2CSharP.LoggerApi.Logger.Interfaces;

namespace L2CSharP.Network.Gameserver
{
    public sealed class GameServerProcessor : IGameServerProcessor
    {
        private readonly IGameLogger _logger;

        public GameServerProcessor(IGameLogger logger)
        {
            _logger = logger;
        }

        public Task HandleServerDisconnectionAsync(GameService gameServer)
        {
            throw new NotImplementedException();
        }

        public Task HandleServerMessageAsync(GameService gameServer, byte[] message)
        {
            throw new NotImplementedException();
        }

        public async Task ProcessGameServerAsync(Socket socket)
        {
            // Implementação do processamento
            await Task.CompletedTask;
        }

        public Task ProcessGameServerAsync(GameService gameServer)
        {
            throw new NotImplementedException();
        }
    }
}