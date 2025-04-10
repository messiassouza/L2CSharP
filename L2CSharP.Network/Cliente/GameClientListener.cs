

using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Cliente.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Net;
using Microsoft.Extensions.Logging;

namespace L2CSharP.Network.Cliente
{


    public class GameClientListener : IDisposable
    {
        private readonly Socket _listener;
        private readonly IGameLogger _logger;
        private readonly IGameClientProcessor _clientProcessor;
        private readonly NetworkSettings _settings;
        private CancellationTokenSource _cts;
        private bool _disposed;

        public GameClientListener(
            IGameLogger logger,
            IGameClientProcessor clientProcessor,
            IOptions<NetworkSettings> networkSettings)
        {
            _logger = logger;
            _clientProcessor = clientProcessor;
            _settings = networkSettings.Value;

            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                ReceiveBufferSize = _settings.SocketBufferSize,
                SendBufferSize = _settings.SocketBufferSize
            };

            _logger.LogInformation("GameClientListener configured for {Address}:{Port}",
                _settings.ClientListenAddr, _settings.ClientListenPort);
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(GameClientListener));

            var localEndPoint = new IPEndPoint(
                IPAddress.Parse(_settings.ClientListenAddr),
                _settings.ClientListenPort);

            _listener.Bind(localEndPoint);
            _listener.Listen(_settings.ClientBacklog);

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _ = Task.Run(() => AcceptClientsAsync(_cts.Token), _cts.Token)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        _logger.LogError(t.Exception, "Client accept loop failed");
                });

            _logger.LogInformation("Started listening for game clients on {EndPoint}", localEndPoint);
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var clientSocket = await _listener.AcceptAsync(cancellationToken);
                    _ = _clientProcessor.ProcessClientAsync(clientSocket)
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                                _logger.LogError(t.Exception, "Error processing client");
                        });
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Client accept loop stopped");
                    break;
                }
                catch (SocketException ex)
                {
                    _logger.LogError(ex, "Socket error in accept loop");
                    await Task.Delay(_settings.ErrorRetryDelayMs, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in client accept loop");
                    await Task.Delay(_settings.ErrorRetryDelayMs, cancellationToken);
                }

                await Task.Delay(_settings.AcceptLoopDelayMs, cancellationToken);
            }
        }

        public async Task StopAsync()
        {
            if (_disposed) return;

            _logger.LogInformation("Stopping GameClientListener...");
            _cts?.Cancel();

            try
            {
                _listener.Shutdown(SocketShutdown.Both);
                _listener.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping listener");
            }
            finally
            {
                _disposed = true;
            }
        }

        public void Dispose()
        {
            StopAsync().GetAwaiter().GetResult();
            _listener.Dispose();
            _cts?.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
