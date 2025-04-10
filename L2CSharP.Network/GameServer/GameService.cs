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
using L2CSharP.Network.Gameserver.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace L2CSharP.Network.Gameserver
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using global::L2CSharP.LoggerApi.Logger.Interfaces;
    using global::L2CSharP.Network.Cliente.Interfaces;
    using Microsoft.Extensions.Logging;

    public class GameService : IDisposable
    {
        private readonly Socket _socket;
        private NetworkStream _networkStream;
        private readonly IGameLogger _logger;
        private readonly IGamePacketProcessor _packetProcessor;
        private CancellationTokenSource _cts;
        private bool _disposed;

        public int CurrentPlayers { get; set; }
        public DateTime LastActivity { get; private set; } = DateTime.UtcNow;
        public string RemoteEndPoint => ((IPEndPoint)_socket.RemoteEndPoint).ToString();
        public bool IsConnected => _socket?.Connected == true;

        public GameService(
            Socket socket,
            IGameLogger logger,
            IGamePacketProcessor packetProcessor)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _packetProcessor = packetProcessor ?? throw new ArgumentNullException(nameof(packetProcessor));

            _networkStream = new NetworkStream(_socket);
            _cts = new CancellationTokenSource();

            _ = Task.Run(() => ReadLoopAsync(_cts.Token));
        }

        private async Task ReadLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                var buffer = new byte[8192];

                while (!cancellationToken.IsCancellationRequested && IsConnected)
                {
                    var bytesRead = await _networkStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    if (bytesRead == 0) break; // Conexão fechada

                    LastActivity = DateTime.UtcNow;

                    // Processar o cabeçalho do pacote (primeiros 2 bytes)
                    if (bytesRead >= 2)
                    {
                        var opcodeBuffer = new byte[2];
                        Array.Copy(buffer, opcodeBuffer, 2);

                        var packetType = _packetProcessor.ProcessPacket(opcodeBuffer);
                        if (packetType != null)
                        {
                            await ProcessPacketAsync(packetType, buffer, bytesRead);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Shutdown normal
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in read loop for {RemoteEndPoint}");
            }
            finally
            {
                Disconnect();
            }
        }

        private async Task ProcessPacketAsync(Type packetType, byte[] buffer, int length)
        {
            try
            {
                var packet = (ReceiveBasePacket)Activator.CreateInstance(
                    packetType,
                    _logger,
                    this,
                    buffer);

                await Task.Run(() =>
                {
                    packet.Read();
                    packet.Run();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing packet from {RemoteEndPoint}");
            }
        }

        public async Task SendPacketAsync(SendBasePacket packet)
        {
            if (!IsConnected) return;

            try
            {
                var data = packet.ToByteArray();
                await _networkStream.WriteAsync(data, 0, data.Length);
                LastActivity = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending packet to {RemoteEndPoint}");
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (!_disposed)
            {
                try
                {
                    _cts?.Cancel();
                    _socket?.Shutdown(SocketShutdown.Both);
                    _networkStream?.Close();
                    _socket?.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error disconnecting {RemoteEndPoint}");
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Disconnect();
                _networkStream?.Dispose();
                _socket?.Dispose();
                _cts?.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }


}