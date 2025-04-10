
using L2CSharP.Config;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace L2CSharP.LoggerApi.Logger.Providers
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();
        private readonly LoggerConfig _config;

        public FileLoggerProvider(LoggerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            EnsureLogDirectoryExists();
            ConfigureNLog();
        }

        private void EnsureLogDirectoryExists()
        {
            try
            {
                Directory.CreateDirectory(_config.LogDirectory);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create log directory: {_config.LogDirectory}", ex);
            }
        }

        private void ConfigureNLog()
        {
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget("fileTarget")
            {
                FileName = Path.Combine(_config.LogDirectory, $"{_config.FilePrefix}.log"),
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${exception:format=ToString}",
                ArchiveFileName = Path.Combine(_config.LogDirectory, $"{_config.FilePrefix}_{{#}}.log"),
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Rolling,
                MaxArchiveFiles = _config.MaxArchiveFiles,
                ConcurrentWrites = true,
                KeepFileOpen = true
            };

            config.AddTarget(fileTarget);
            config.AddRuleForAllLevels(fileTarget);

            LogManager.Configuration = config;
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name =>
            {
                var nlogLogger = LogManager.GetLogger(name);
                return new FileLogger(nlogLogger);
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _loggers.Clear();
                LogManager.Shutdown();
            }
        }

        private class FileLogger : Microsoft.Extensions.Logging.ILogger
        {
            private readonly NLog.ILogger _logger;

            public FileLogger(NLog.ILogger logger)
            {
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                if (state == null)
                {
                    return NullScope.Instance;
                }

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
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                var message = formatter(state, exception);
                var nlogLevel = ConvertToNLogLevel(logLevel);

                var logEvent = new LogEventInfo(nlogLevel, _logger.Name, message)
                {
                    Exception = exception
                };

                if (eventId.Id != 0)
                {
                    logEvent.Properties["EventId"] = eventId;
                }

                if (state != null)
                {
                    logEvent.Properties["State"] = state;
                }

                _logger.Log(logEvent);
            }

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

            private class NullScope : IDisposable
            {
                public static NullScope Instance { get; } = new NullScope();
                public void Dispose() { }
            }
        }
    }
}