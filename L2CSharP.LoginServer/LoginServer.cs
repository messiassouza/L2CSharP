using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Cliente;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.LoginServer
{

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Net.Sockets;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using global::L2CSharP.Network.Cliente.Interfaces;
    using global::L2CSharP.Network;
    using global::L2CSharP.DataBase;

    namespace L2CSharP.LoginServer
    {
        public class LoginServer : IHostedService, IDisposable
        {
            private readonly IGameLogger _logger;
            private readonly AppDbContext _dbContext;
            private readonly IServiceProvider _serviceProvider;
            private readonly IOptions<NetworkSettings> _networkSettings;
            private readonly IChronicleProcessor _plugins;
            private readonly IGameClientProcessor _clientProcessor;
            private readonly IGameServerProcessor _serverProcessor;
            private readonly IPacketProcessor _packetProcessor;
            private readonly IGamePacketProcessor _gamePacketProcessor;

            private GameClientListener _gameClientListener;
            private GameServerListener _gameServerListener;
            private IMaxConnections _maxConnections;

            private bool _isInitialized;
            private bool _isRunning;
            private bool _disposed;

            public LoginServer(
                IGameLogger logger,
                AppDbContext dbContext,
                IServiceProvider serviceProvider,
                IOptions<NetworkSettings> networkSettings,
                IChronicleProcessor plugins,
                IGameClientProcessor clientProcessor,
                IGameServerProcessor serverProcessor,
                IPacketProcessor packetProcessor,
                IGamePacketProcessor gamePacketProcessor)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
                _networkSettings = networkSettings ?? throw new ArgumentNullException(nameof(networkSettings));
                _plugins = plugins ?? throw new ArgumentNullException(nameof(plugins));
                _clientProcessor = clientProcessor ?? throw new ArgumentNullException(nameof(clientProcessor));
                _serverProcessor = serverProcessor ?? throw new ArgumentNullException(nameof(serverProcessor));
                _packetProcessor = packetProcessor ?? throw new ArgumentNullException(nameof(packetProcessor));
                _gamePacketProcessor = gamePacketProcessor ?? throw new ArgumentNullException(nameof(gamePacketProcessor));

                _logger.Debug("LoginServer instance created");
            }

            public async Task StartAsync(CancellationToken cancellationToken)
            {
                try
                {
                    await InitializeAsync();
                    await StartServerAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to start LoginServer: {ex.Message}");
                    throw;
                }
            }

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                await StopServerAsync();
            }

            public async Task InitializeAsync()
            {
                if (_isInitialized) return;

                try
                {
                    _logger.Debug("Initializing LoginServer...");

                    await InitializeDatabaseAsync();
                    InitializeNetworkComponents();
                    await InitializePluginsAsync();

                    _isInitialized = true;
                    _logger.LogInformation("LoginServer initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Initialization failed: {ex.Message}");
                    throw new Exception("Failed to initialize LoginServer", ex);
                }
            }

            public async Task InitializeDatabaseAsync()
            {
                _logger.Debug("Initializing database...");

                try
                {
                    if (!await _dbContext.Database.CanConnectAsync())
                    {
                        throw new Exception("Could not connect to database");
                    }

                    await _dbContext.Database.MigrateAsync();
                    await LoadInitialDataAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Database initialization failed: {ex.Message}");
                    throw;
                }
            }

            private async Task LoadInitialDataAsync()
            {
                _logger.Debug("Loading initial data...");
                // Implemente a lógica de carregamento de dados iniciais aqui
                await Task.CompletedTask;
            }

            private void InitializeNetworkComponents()
            {
                _logger.Debug("Initializing network components...");

                try
                {
                    _maxConnections = _serviceProvider.GetRequiredService<IMaxConnections>();

                    _packetProcessor.Initialize();
                    _gamePacketProcessor.Initialize();

                    _gameServerListener = new GameServerListener(
                        _logger,
                        _serverProcessor,
                        _gamePacketProcessor,
                        _networkSettings.Value);

                    _gameClientListener = new GameClientListener(
                        _logger,
                        _clientProcessor,
                        _networkSettings.Value);

                    _logger.Debug("Network components initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Network components initialization failed: {ex.Message}");
                    throw new Exception("Failed to initialize network components", ex);
                }
            }

            private async Task InitializePluginsAsync()
            {
                _logger.Debug("Initializing plugins...");
                // Implemente a lógica de inicialização de plugins aqui
                await Task.CompletedTask;
            }

            private async Task StartServerAsync()
            {
                if (_isRunning) return;

                try
                {
                    _logger.Debug("Starting LoginServer listeners...");

                    await Task.WhenAll(
                        _gameClientListener.StartAsync(),
                        _gameServerListener.StartAsync()
                    );

                    _isRunning = true;
                    _logger.LogInformation("LoginServer started successfully");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error starting LoginServer: {ex.Message}");
                    throw;
                }
            }

            private async Task StopServerAsync()
            {
                if (!_isRunning) return;

                try
                {
                    _logger.Debug("Stopping LoginServer...");

                    await Task.WhenAll(
                        _gameClientListener.StopAsync(),
                        _gameServerListener.StopAsync()
                    );

                    _isRunning = false;
                    _logger.LogInformation("LoginServer stopped successfully");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error stopping LoginServer: {ex.Message}");
                    throw;
                }
            }

            public void Dispose()
            {
                if (_disposed) return;

                try
                {
                    _logger.Debug("Disposing LoginServer resources...");

                    if (_isRunning)
                    {
                        StopServerAsync().GetAwaiter().GetResult();
                    }

                    _gameClientListener?.Dispose();
                    _gameServerListener?.Dispose();

                    _disposed = true;
                    _logger.Debug("LoginServer resources disposed");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error during disposal: {ex.Message}");
                }
                finally
                {
                    GC.SuppressFinalize(this);
                }
            }
        }

        public class GameClientListener : IDisposable
        {
            private readonly IGameLogger _logger;
            private readonly IGameClientProcessor _clientProcessor;
            private readonly NetworkSettings _networkSettings;
            private Socket _listener;
            private bool _disposed;

            public GameClientListener(
                IGameLogger logger,
                IGameClientProcessor clientProcessor,
                NetworkSettings networkSettings)
            {
                _logger = logger;
                _clientProcessor = clientProcessor;
                _networkSettings = networkSettings;
            }

            public async Task StartAsync()
            {
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Parse(_networkSettings.ClientListenAddr), _networkSettings.ClientListenPort);
                _listener.Bind(endPoint);
                _listener.Listen(_networkSettings.ClientBacklog);
                _logger.LogInformation("Client listener started on {endPoint}", endPoint);
                await Task.CompletedTask;
            }

            public async Task StopAsync()
            {
                _listener?.Close();
                await Task.CompletedTask;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _listener?.Dispose();
                _disposed = true;
            }
        }

        public class GameServerListener : IDisposable
        {
            private readonly IGameLogger _logger;
            private readonly IGameServerProcessor _serverProcessor;
            private readonly IGamePacketProcessor _gamePacketProcessor;
            private readonly NetworkSettings _networkSettings;
            private Socket _listener;
            private bool _disposed;

            public GameServerListener(
                IGameLogger logger,
                IGameServerProcessor serverProcessor,
                IGamePacketProcessor gamePacketProcessor,
                NetworkSettings networkSettings)
            {
                _logger = logger;
                _serverProcessor = serverProcessor;
                _gamePacketProcessor = gamePacketProcessor;
                _networkSettings = networkSettings;
            }

            public async Task StartAsync()
            {
                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Parse(_networkSettings.GameServerListenAddr), _networkSettings.GameServerListenPort);
                _listener.Bind(endPoint);
                _listener.Listen(_networkSettings.GameServerBacklog);
                _logger.LogInformation("Game server listener started on {endPoint}", endPoint);
                await Task.CompletedTask;
            }

            public async Task StopAsync()
            {
                _listener?.Close();
                await Task.CompletedTask;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _listener?.Dispose();
                _disposed = true;
            }
        }
    }

}
