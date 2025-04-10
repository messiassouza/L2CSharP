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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace L2CSharP.Network.Gameserver
{
    using L2CSharP.LoggerApi.Logger.Interfaces;
    using L2CSharP.Network.Cliente.Interfaces;
    using L2CSharP.Network.Gameserver.Packets;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System.Net;
    using System.Net.Sockets;

    public sealed class GameServerListener : IDisposable
    {
        private static GameServerListener _instance;
        private static readonly object _padlock = new();
        private readonly Socket _listener;
        private readonly IGameLogger _logger;
        private readonly IGameServerProcessor _gameServerProcessor;
        private readonly NetworkSettings _networkSettings;
        private readonly IGamePacketProcessor _gamePacketProcessor;
        private CancellationTokenSource _acceptLoopTokenSource;
        private bool _disposed;

        public static GameServerListener Instance(
            IGameLogger logger,
            IGameServerProcessor gameServerProcessor,
             IGamePacketProcessor gamePacketProcessor,
            IOptions<NetworkSettings> networkSettings)
        {
            lock (_padlock)
            {
                if (_instance == null || _instance._disposed)
                {
                    _instance = new GameServerListener(
                        logger,
                        gameServerProcessor,
                        gamePacketProcessor,
                        networkSettings.Value);
                }
                return _instance;
            }
        }

        private GameServerListener(
            IGameLogger logger,
            IGameServerProcessor gameServerProcessor,
               IGamePacketProcessor gamePacketProcessor,
            NetworkSettings networkSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _gameServerProcessor = gameServerProcessor ?? throw new ArgumentNullException(nameof(gameServerProcessor));
            _networkSettings = networkSettings ?? throw new ArgumentNullException(nameof(networkSettings));
            _gamePacketProcessor = gamePacketProcessor ?? throw new ArgumentNullException(nameof(gamePacketProcessor));
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                LingerState = new LingerOption(false, 0)
            };

            _acceptLoopTokenSource = new CancellationTokenSource();

            _logger.LogInformation("GameServerListener initialized for {Address}:{Port}",
                _networkSettings.GameServerListenAddr,
                _networkSettings.GameServerListenPort);
        }

        public async Task<bool> StartListeningAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(GameServerListener));

            try
            {
                var ipAddress = IPAddress.Parse(_networkSettings.GameServerListenAddr);
                var endPoint = new IPEndPoint(ipAddress, _networkSettings.GameServerListenPort);

                _listener.Bind(endPoint);
                _listener.Listen(_networkSettings.GameServerBacklog);

                _logger.LogInformation("Started listening for game servers on {EndPoint}", endPoint);

                _ = Task.Run(() => RunAcceptLoopAsync(_acceptLoopTokenSource.Token))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            _logger.LogError(t.Exception, "Game server accept loop crashed");
                        }
                    });

                return true;
            }
            catch (SocketException ex)
            {
                _logger.LogError(ex, "Socket error while starting game server listener");
                await StopListeningAsync();
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while starting game server listener");
                await StopListeningAsync();
                return false;
            }
        }

        private async Task RunAcceptLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var acceptedSocket = await _listener.AcceptAsync(cancellationToken);
                    var remoteIp = ((IPEndPoint)acceptedSocket.RemoteEndPoint).Address.ToString();

                    _logger.LogInformation("Incoming game server connection from {RemoteIP}", remoteIp);

                    _ = ProcessGameServerAsync(acceptedSocket, remoteIp);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Game server accept loop stopped (normal shutdown)");
                    break;
                }
                catch (SocketException ex)
                {
                    _logger.LogError( "Socket error in game server accept loop", ex);
                    await Task.Delay(1000, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in game server accept loop");
                    await Task.Delay(1000, cancellationToken);
                }
            }
        }

        private async Task ProcessGameServerAsync(Socket socket, string remoteIp)
        {
            try
            {
                var gameServer = new GameService(socket, _logger, _gamePacketProcessor);
                await _gameServerProcessor.ProcessGameServerAsync(gameServer);
            }
            catch (Exception ex)
            {
                _logger.LogError( $"Error processing game server {remoteIp}", ex);
                SafeDisconnect(socket);
            }
        }

        public async Task StopListeningAsync()
        {
            if (_disposed) return;

            _logger.LogInformation("Stopping GameServerListener...");

            try
            {
                _acceptLoopTokenSource.Cancel();
                _listener.Shutdown(SocketShutdown.Both);
                _listener.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while stopping game server listener", ex);
            }
            finally
            {
                _disposed = true;
            }
        }

        private void SafeDisconnect(Socket socket)
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
                socket.Close();
                socket.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while disconnecting game server", ex);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                StopListeningAsync().GetAwaiter().GetResult();
                _acceptLoopTokenSource.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        ~GameServerListener()
        {
            Dispose();
        }
    }

}