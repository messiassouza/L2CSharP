 
 
using L2CodCraft.LoggerApi.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using L2CodCraft.LoggerApi.Logger.L2CodCraft.LoggerApi.Logger;
using L2CSharP.Config;
using L2CSharP.LoggerApi.Providers;
using L2CSharP.LoggerApi.Logger.Providers;
using L2CSharP.LoggerApi.Logger.Interfaces; 

using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.LoggerApi.Logger.Providers;
using L2CSharP.LoggerApi.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2CSharP.LoggerApi.Providers; 
using L2CSharP.Config;

namespace L2CSharP.LoggerApi.Extensions
{
    public static class LoggerServiceExtensions
    {
        public static IServiceCollection AddGameLogger(this IServiceCollection services, Action<LoggerConfig> configure)
        {
            var config = new LoggerConfig();
            configure(config);

            services.AddSingleton(config);

            if (config.EnableConsoleLogging)
                services.AddSingleton<ILoggerProvider, ConsoleLoggerProvider>();

            if (config.EnableFileLogging)
                services.AddSingleton<ILoggerProvider, FileLoggerProvider>();

            services.AddSingleton<IGameLogger, GameLogger>();

            return services;
        }
    }
}
