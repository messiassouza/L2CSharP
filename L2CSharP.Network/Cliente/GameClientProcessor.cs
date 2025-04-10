namespace L2CSharP.Network.Cliente
{


    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using L2CSharP.LoggerApi.Logger.Interfaces;
    using L2CSharP.Network.Cliente.Interfaces;
    using L2CSharP.Network.Crypt;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public sealed class GameClientProcessor : IGameClientProcessor, IDisposable, IAsyncDisposable
    {
        private readonly IGameLogger _logger;
        private readonly IMaxConnections _maxConnections;
        private readonly IPacketProcessor _packetProcessor;
        private readonly NetworkSettings _networkSettings;
        private readonly ConcurrentDictionary<Guid, GameClient> _clients = new();
        private readonly byte[][] _blowfishKeys;
        private readonly ScrambledKeyPair[] _scrambledPairs;
        private readonly FloodProtector _connectionFloodProtector;
        private bool _disposed;
        private readonly RNGCryptoServiceProvider _rng = new();

        public int ConnectedClients => _clients.Count;
        public IReadOnlyCollection<GameClient> Clients => _clients.Values.ToList().AsReadOnly();

        public int ConnectedClientCount => throw new NotImplementedException();

        public GameClientProcessor(
            IPacketProcessor packetProcessor,
            IGameLogger logger,
            IMaxConnections maxConnections,
            IOptions<NetworkSettings> networkSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _maxConnections = maxConnections ?? throw new ArgumentNullException(nameof(maxConnections));
            _packetProcessor = packetProcessor ?? throw new ArgumentNullException(nameof(packetProcessor));
            _networkSettings = networkSettings?.Value ?? throw new ArgumentNullException(nameof(networkSettings));

            // Gerar chaves criptográficas
            _blowfishKeys = GenerateBlowfishKeys(_networkSettings.BlowfishKeyCount);
            _scrambledPairs = GenerateScrambledPairs(_networkSettings.ScrambledPairCount);

            _connectionFloodProtector = new FloodProtector(
                _networkSettings.FloodProtectionLimit,
                _networkSettings.FloodProtectionInterval);

            _logger.LogInformation("GameClientProcessor initialized with {KeyCount} Blowfish keys and {PairCount} scrambled pairs",
                _networkSettings.BlowfishKeyCount, _networkSettings.ScrambledPairCount);
        }

        public async Task ProcessClientAsync(Socket clientSocket, CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(GameClientProcessor));

            if (clientSocket == null || !clientSocket.Connected)
                return;

            IPEndPoint remoteEndPoint = null;
            try
            {
                remoteEndPoint = (IPEndPoint)clientSocket.RemoteEndPoint;

                // Verificação de flood
                if (!_connectionFloodProtector.HandleFlood(remoteEndPoint))
                {
                    _logger.LogWarning("Connection flood detected from {IP}", remoteEndPoint.Address);
                    SafeDisconnect(clientSocket);
                    return;
                }

                _logger.LogInformation("New connection from {IP}:{Port}",
                    remoteEndPoint.Address, remoteEndPoint.Port);

                // Pequeno atraso para mitigar ataques
                await Task.Delay(_networkSettings.ConnectionDelay, cancellationToken);

                // Verificar conexões máximas
                if (!_maxConnections.AcceptConnection(remoteEndPoint.Address.ToString()))
                {
                    _logger.LogWarning("Max connections reached for {IP}", remoteEndPoint.Address);
                    SafeDisconnect(clientSocket);
                    return;
                }

                // Criar novo cliente
                var gameClient = new GameClient(
                    clientSocket,
                    _logger,
                    this,
                    GenerateBlowfishKey(),
                    GenerateScrambledPair(),
                    _packetProcessor,
                    _networkSettings.FloodProtectionLimit,
                    _networkSettings.FloodProtectionInterval);

                gameClient.Disconnected += OnClientDisconnected;

                if (_clients.TryAdd(gameClient.Id, gameClient))
                {
                    await gameClient.StartProcessingAsync(cancellationToken);
                    _logger.LogDebug("Client {Id} added successfully", gameClient.Id);
                }
                else
                {
                    _logger.LogError("Failed to add client to dictionary");
                    SafeDisconnect(clientSocket);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Client processing canceled for {IP}", remoteEndPoint?.Address);
                SafeDisconnect(clientSocket);
            }
            catch (SocketException sex)
            {
                _logger.LogWarning("Socket error from {IP}: {Message}",
                    remoteEndPoint?.Address, sex.Message);
                SafeDisconnect(clientSocket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing client from {IP}", remoteEndPoint?.Address);
                SafeDisconnect(clientSocket);
            }
        }

        private void OnClientDisconnected(object sender, EventArgs e)
        {
            if (sender is GameClient client)
            {
                try
                {
                    if (_clients.TryRemove(client.Id, out _))
                    {
                        _maxConnections.Disconnect(client.RemoteEndPoint?.Address?.ToString());
                        _logger.LogInformation("Client {Id} disconnected from {IP}",
                            client.Id, client.RemoteEndPoint?.Address);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling disconnection for client {Id}", client.Id);
                }
            }
        }

        private byte[][] GenerateBlowfishKeys(int count)
        {
            var keys = new byte[count][];
            for (int i = 0; i < count; i++)
            {
                keys[i] = new byte[16];
                _rng.GetBytes(keys[i]);
                _logger.LogTrace("Generated Blowfish key {Index}: {Key}",
                    i, BitConverter.ToString(keys[i]));
            }
            return keys;
        }

        private ScrambledKeyPair[] GenerateScrambledPairs(int count)
        {
            var pairs = new ScrambledKeyPair[count];
            for (int i = 0; i < count; i++)
            {
                pairs[i] = ScrambledKeyPair.Generate();
                _logger.LogTrace("Generated ScrambledKeyPair {Index}", i);
            }
            return pairs;
        }

        public byte[] GenerateBlowfishKey()
        {
            var index = RandomNumberGenerator.GetInt32(_blowfishKeys.Length);
            var key = _blowfishKeys[index];
            _logger.LogTrace("Selected Blowfish key {Index}: {Key}",
                index, BitConverter.ToString(key));
            return key;
        }

        public ScrambledKeyPair GenerateScrambledPair()
        {
            var index = RandomNumberGenerator.GetInt32(_scrambledPairs.Length);
            var pair = _scrambledPairs[index];
            _logger.LogTrace("Selected ScrambledKeyPair {Index}", index);
            return pair;
        }

        private static void SafeDisconnect(Socket socket)
        {
            try
            {
                socket?.Shutdown(SocketShutdown.Both);
                socket?.Close();
                socket?.Dispose();
            }
            catch { /* Ignorar erros de desconexão */ }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            _logger.LogInformation("Disposing GameClientProcessor with {Count} active clients", _clients.Count);

            var disposalTasks = new List<Task>();
            foreach (var client in _clients.Values)
            {
                disposalTasks.Add(Task.Run(() =>
                {
                    try
                    {
                        client.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error disposing client {Id}", client.Id);
                    }
                }));
            }

            await Task.WhenAll(disposalTasks);
            _clients.Clear();
            _rng.Dispose();

            _logger.LogInformation("GameClientProcessor disposed successfully");
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        public Task ProcessClientAsync(Socket client)
        {
            throw new NotImplementedException();
        }
    }
     



}
