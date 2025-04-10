
using L2CSharP.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace L2CSharP.LoggerApi.Providers
{
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers = new();
        private readonly LoggerConfig _config;
        private readonly ConsoleColor _defaultColor;

        public ConsoleLoggerProvider(LoggerConfig config)
        {
            _config = config;
            _defaultColor = Console.ForegroundColor;
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ConsoleLogger(name, _config, _defaultColor));
        }

        public void Dispose()
        {
            _loggers.Clear();
            Console.ResetColor();
            GC.SuppressFinalize(this);
        }

        private class ConsoleLogger : Microsoft.Extensions.Logging.ILogger
        {
            private readonly string _name;
            private readonly LoggerConfig _config;
            private readonly ConsoleColor _defaultColor;

            public ConsoleLogger(string name, LoggerConfig config, ConsoleColor defaultColor)
            {
                _name = name;
                _config = config;
                _defaultColor = defaultColor;
            }

            public IDisposable BeginScope<TState>(TState state) => null;

            public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
            {
                // Correção: Converter para valores numéricos antes de comparar
                return ConvertLogLevelToInt(logLevel) >= _config.ConsoleLogLevel.Ordinal;
            }

            public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel,
                                   EventId eventId,
                                   TState state,
                                   Exception exception,
                                   Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;

                var message = formatter(state, exception);
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                var logLevelString = GetLogLevelString(logLevel);

                SetConsoleColor(logLevel, message);
                Console.WriteLine($"{timestamp} {logLevelString} [{_name}]: {message}");
                Console.ForegroundColor = _defaultColor;
            }

            private int ConvertLogLevelToInt(Microsoft.Extensions.Logging.LogLevel logLevel)
            {
                return logLevel switch
                {
                    Microsoft.Extensions.Logging.LogLevel.Trace => 0,
                    Microsoft.Extensions.Logging.LogLevel.Debug => 1,
                    Microsoft.Extensions.Logging.LogLevel.Information => 2,
                    Microsoft.Extensions.Logging.LogLevel.Warning => 3,
                    Microsoft.Extensions.Logging.LogLevel.Error => 4,
                    Microsoft.Extensions.Logging.LogLevel.Critical => 5,
                    Microsoft.Extensions.Logging.LogLevel.None => 6,
                    _ => 2 // Default to Information
                };
            }

            private void SetConsoleColor(Microsoft.Extensions.Logging.LogLevel logLevel, string message)
            {
                // Cores padrão por nível de log
                Console.ForegroundColor = logLevel switch
                {
                    Microsoft.Extensions.Logging.LogLevel.Error => ConsoleColor.Red,
                    Microsoft.Extensions.Logging.LogLevel.Warning => ConsoleColor.Yellow,
                    Microsoft.Extensions.Logging.LogLevel.Information => ConsoleColor.White,
                    Microsoft.Extensions.Logging.LogLevel.Debug => ConsoleColor.Gray,
                    Microsoft.Extensions.Logging.LogLevel.Trace => ConsoleColor.DarkGray,
                    _ => _defaultColor
                };

                // Cores customizadas para padrões de mensagem do Lineage 2
                if (message.Contains(">>"))
                {
                    Console.ForegroundColor = message switch
                    {
                        string s when s.Contains("GS>>") => ConsoleColor.Cyan,
                        string s when s.Contains("LS>>") => ConsoleColor.Blue,
                        string s when s.Contains("CLIENT>>") => ConsoleColor.Green,
                        string s when s.Contains("AUTH>>") => ConsoleColor.Magenta,
                        _ => Console.ForegroundColor
                    };
                }
            }

            private string GetLogLevelString(Microsoft.Extensions.Logging.LogLevel logLevel)
            {
                return logLevel switch
                {
                    Microsoft.Extensions.Logging.LogLevel.Trace => "TRACE",
                    Microsoft.Extensions.Logging.LogLevel.Debug => "DEBUG",
                    Microsoft.Extensions.Logging.LogLevel.Information => "INFO ",
                    Microsoft.Extensions.Logging.LogLevel.Warning => "WARN ",
                    Microsoft.Extensions.Logging.LogLevel.Error => "ERROR",
                    Microsoft.Extensions.Logging.LogLevel.Critical => "CRIT ",
                    _ => "     "
                };
            }
        }
    }
}