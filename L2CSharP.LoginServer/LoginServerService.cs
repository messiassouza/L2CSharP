 

namespace L2CSharP.LoginServer
{

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using global::L2CSharP.LoggerApi.Logger.Interfaces;
    using global::L2CSharP.LoginServer.L2CSharP.LoginServer;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class LoginServerService : IHostedService, IDisposable
    {
        private readonly LoginServer _loginServer;
        private readonly IGameLogger _logger;
        private bool _disposed;

        public LoginServerService(LoginServer loginServer, IGameLogger logger)
        {
            _loginServer = loginServer ?? throw new ArgumentNullException(nameof(loginServer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.Debug("Initializing login server...");

                // Usando InitializeAsync() conforme sua classe original
                await _loginServer.InitializeAsync();

                _logger.Debug("Starting login server...");

                // Usando StartAsync() conforme sua classe original
                await _loginServer.StartAsync(cancellationToken);

                _logger.LogInformation("Login server started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login server initialization failed");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down login server...");
            try
            {
                // Usando StopAsync() conforme sua classe original
                await _loginServer.StopAsync(cancellationToken);
                _logger.LogInformation("Login server stopped successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login server shutdown");
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                _loginServer.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing login server");
            }
            finally
            {
                _disposed = true;
            }
        }
    }

}
