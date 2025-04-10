using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Network.Cliente
{
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Channels;
    using L2CSharP.LoggerApi.Logger.Interfaces;
    using L2CSharP.Network.Cliente.Interfaces;
    using L2CSharP.Network.Cliente.Packets;
    using L2CSharP.Network.Crypt;
    using Microsoft.Extensions.Logging;



    public class GameClient : IDisposable
    {
        private readonly Socket _socket;
        private readonly NetworkStream _stream;
        private readonly CryptEngine _loginCrypt;
        private readonly FloodProtector _floodProtector;
        private readonly IGameLogger _logger;
        private readonly IGameClientProcessor _clientProcessor;
        private readonly Channel<SendBasePacket> _sendQueue;
        private readonly CancellationTokenSource _cts;
        private readonly IPacketProcessor _packetProcessor;
        private bool _disposed;

        public event EventHandler? Disconnected;
        public Guid Id { get; } = Guid.NewGuid();
        public string? CurrentAccount { get; set; }
        public IPEndPoint? RemoteEndPoint => _socket.RemoteEndPoint as IPEndPoint;
        public byte[] BlowfishKey { get; }
        public ScrambledKeyPair ScrambledPair { get; }

        public GameClient(
            Socket socket,
            IGameLogger logger,
            IGameClientProcessor clientProcessor,
            byte[] blowfishKey,
            ScrambledKeyPair scrambledPair,
            IPacketProcessor packetProcessor,
            int floodProtectionLimit = 50,
            int floodProtectionInterval = 1000)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientProcessor = clientProcessor ?? throw new ArgumentNullException(nameof(clientProcessor));
            _stream = new NetworkStream(_socket, ownsSocket: true);
            _loginCrypt = new CryptEngine();
            BlowfishKey = blowfishKey;
            ScrambledPair = scrambledPair;
            _floodProtector = new FloodProtector(floodProtectionLimit, floodProtectionInterval);
            _sendQueue = Channel.CreateUnbounded<SendBasePacket>();
            _cts = new CancellationTokenSource();
            _packetProcessor = packetProcessor;
            _loginCrypt.updateKey(BlowfishKey);

            _ = Task.Run(() => StartProcessingAsync(_cts.Token));
        }

        public async Task StartProcessingAsync(CancellationToken cancellationToken)
        {
            var receiveTask = ReceivePacketsAsync(cancellationToken);
            var sendTask = SendPacketsAsync(cancellationToken);

            await Task.WhenAny(receiveTask, sendTask);
            await DisposeAsync();
        }

        private async Task ReceivePacketsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _socket.Connected)
                {
                    byte[] sizeBuffer = new byte[2];
                    int bytesRead = await _stream.ReadAsync(sizeBuffer, 0, 2, cancellationToken);

                    if (bytesRead == 0) break; // Connection closed

                    short payloadSize = BitConverter.ToInt16(sizeBuffer, 0);
                    _logger.LogDebug("Received packet with size: {PayloadSize}", payloadSize);

                    if (!_floodProtector.CanProcess(RemoteEndPoint))
                    {
                        _logger.LogWarning("Flood protection triggered for {RemoteEndPoint}", RemoteEndPoint);
                        break;
                    }

                    byte[] payloadBuffer = new byte[payloadSize - 2];
                    bytesRead = await _stream.ReadAsync(payloadBuffer, 0, payloadBuffer.Length, cancellationToken);

                    if (bytesRead != payloadBuffer.Length)
                    {
                        _logger.LogWarning("Incomplete packet received from {RemoteEndPoint}", RemoteEndPoint);
                        break;
                    }

                    if (!_loginCrypt.Decrypt(payloadBuffer))
                    {
                        _logger.LogWarning("Invalid checksum from {RemoteEndPoint}", RemoteEndPoint);
                        break;
                    }

                    ProcessPacket(payloadBuffer);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving packets from {RemoteEndPoint}", RemoteEndPoint);
            }
            finally
            {
                await DisposeAsync();
            }
        }

        private void ProcessPacket(byte[] buffer)
        {
            try
            {
                Type? packetType = _packetProcessor.ProcessPacket(buffer);
                if (packetType != null)
                {
                    if (Activator.CreateInstance(packetType, this, buffer) is ReceiveBasePacket packet)
                    {
                        Task.Run(() => packet.Run());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing packet from {RemoteEndPoint}", RemoteEndPoint);
            }
        }

        private async Task SendPacketsAsync(CancellationToken cancellationToken)
        {
            try
            {
                await foreach (var packet in _sendQueue.Reader.ReadAllAsync(cancellationToken))
                {
                    packet.Write();
                    byte[] encrypted = _loginCrypt.Encrypt(packet.ToByteArray());

                    byte[] size = BitConverter.GetBytes((short)(encrypted.Length + 2));
                    byte[] fullPacket = new byte[size.Length + encrypted.Length];

                    Buffer.BlockCopy(size, 0, fullPacket, 0, size.Length);
                    Buffer.BlockCopy(encrypted, 0, fullPacket, size.Length, encrypted.Length);

                    await _stream.WriteAsync(fullPacket, 0, fullPacket.Length, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending packets to {RemoteEndPoint}", RemoteEndPoint);
            }
        }

        public async Task SendPacketAsync(SendBasePacket packet)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(GameClient));
            await _sendQueue.Writer.WriteAsync(packet);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;

            _cts.Cancel();

            try
            {
                _sendQueue.Writer.Complete();
                _stream.Dispose();
                _socket.Dispose();
                Disconnected?.Invoke(this, EventArgs.Empty);
                _logger.LogInformation("Client disconnected: {RemoteEndPoint}", RemoteEndPoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing client {RemoteEndPoint}", RemoteEndPoint);
            }
            finally
            {
                _disposed = true;
                _cts.Dispose();
            }
        }

        public void Dispose() => DisposeAsync().AsTask().Wait();
    }
}
