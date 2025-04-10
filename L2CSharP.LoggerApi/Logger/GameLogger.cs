

using L2CSharP.LoggerApi.Logger.Interfaces;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Diagnostics;

namespace L2CodCraft.LoggerApi.Logger
{
    using global::L2CSharP.Config;
    using Microsoft.Extensions.Logging;
    using NLog;
    using System;
    using System.Diagnostics;

    namespace L2CodCraft.LoggerApi.Logger
    {
        public class GameLogger : IGameLogger, Microsoft.Extensions.Logging.ILogger
        {
            private readonly NLog.ILogger _logger;
            private readonly LoggerConfig _config;

            public GameLogger(LoggerConfig config)
            {
                _config = config ?? throw new ArgumentNullException(nameof(config));
                _logger = LogManager.GetCurrentClassLogger();

                // Initialize log directory if specified
                if (!string.IsNullOrEmpty(_config.LogDirectory))
                {
                    Directory.CreateDirectory(_config.LogDirectory);
                }
            }

            #region ILogger Implementation
            public IDisposable BeginScope<TState>(TState state)
            {
                return NLog.NestedDiagnosticsLogicalContext.Push(state);
            }

            public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
            {
                return _logger.IsEnabled(ConvertToNLogLevel(logLevel));
            }

            public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel,
                                  EventId eventId,
                                  TState state,
                                  Exception exception,
                                  Func<TState, Exception, string> formatter)
            {
                var nlogLevel = ConvertToNLogLevel(logLevel);
                if (!_logger.IsEnabled(nlogLevel))
                    return;

                var message = formatter(state, exception);
                var logEvent = new LogEventInfo(nlogLevel, _logger.Name, message)
                {
                    Exception = exception,
                    Properties =
                {
                    ["EventId"] = eventId,
                    ["State"] = state
                }
                };

                _logger.Log(logEvent);
            }
            #endregion

            #region IGameLogger Implementation
            public void Debug(string message) => _logger.Debug(message);
            public void Info(string message) => _logger.Info(message);
            public void Warn(string message) => _logger.Warn(message);
            public void Error(string message) => _logger.Error(message);
            public void Error(string message, Exception e) => _logger.Error(e, message);
            public void Critical(string message) => _logger.Fatal(message);

            public void ErrorTrace(string message, int framesToSkip = 2)
            {
                var stackTrace = new StackTrace(framesToSkip, true);
                var frame = stackTrace.GetFrame(0);
                var traceInfo = $"[{frame?.GetMethod()?.DeclaringType?.Name}.{frame?.GetMethod()?.Name}:{frame?.GetFileLineNumber()}]";
                _logger.Error($"{traceInfo} {message}");
            }

            public void NetworkLog(string message) => _logger.Info($"[NETWORK] {message}");
            public void GameServerLog(string message) => _logger.Info($"[GAMESERVER] {message}");
            public void LoginServerLog(string message) => _logger.Info($"[LOGINSERVER] {message}");
            public void AuthLog(string message) => _logger.Info($"[AUTH] {message}");

            public void PacketDump(byte[] data, string direction, string description)
            {
                if (data == null || data.Length == 0)
                    return;

                try
                {
                    var hexDump = BitConverter.ToString(data).Replace("-", " ");
                    _logger.Debug($"[PACKET] {direction} {description}\n{hexDump}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to log packet dump");
                }
            }
            #endregion

            #region Helper Methods

            private static NLog.LogLevel ConvertToNLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel)
            {
                return logLevel switch
                {
                    Microsoft.Extensions.Logging.LogLevel.Trace => NLog.LogLevel.Trace,
                    Microsoft.Extensions.Logging.LogLevel.Debug => NLog.LogLevel.Debug,
                    Microsoft.Extensions.Logging.LogLevel.Information => NLog.LogLevel.Info,
                    Microsoft.Extensions.Logging.LogLevel.Warning => NLog.LogLevel.Warn,
                    Microsoft.Extensions.Logging.LogLevel.Error => NLog.LogLevel.Error,
                    Microsoft.Extensions.Logging.LogLevel.Critical => NLog.LogLevel.Fatal,
                    _ => NLog.LogLevel.Off
                };
            }

            private static Microsoft.Extensions.Logging.LogLevel ConvertToMsLogLevel(NLog.LogLevel logLevel)
            {
                if (logLevel == NLog.LogLevel.Trace) return Microsoft.Extensions.Logging.LogLevel.Trace;
                if (logLevel == NLog.LogLevel.Debug) return Microsoft.Extensions.Logging.LogLevel.Debug;
                if (logLevel == NLog.LogLevel.Info) return Microsoft.Extensions.Logging.LogLevel.Information;
                if (logLevel == NLog.LogLevel.Warn) return Microsoft.Extensions.Logging.LogLevel.Warning;
                if (logLevel == NLog.LogLevel.Error) return Microsoft.Extensions.Logging.LogLevel.Error;
                if (logLevel == NLog.LogLevel.Fatal) return Microsoft.Extensions.Logging.LogLevel.Critical;
                return Microsoft.Extensions.Logging.LogLevel.None;
            }

            #endregion
        }
    }
}