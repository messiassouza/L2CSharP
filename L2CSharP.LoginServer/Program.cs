using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;
using L2CSharP.LoggerApi.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using L2CSharP.LoggerApi.Logger.Interfaces;
using System.IO;
using L2CodCraft.LoggerApi.Logger.L2CodCraft.LoggerApi.Logger;
using NLog;
using L2CSharP.LoginServer;
using L2CSharP.DataBase;
using L2CSharP.Network.Cliente.Interfaces;
using Microsoft.EntityFrameworkCore;
using L2CSharP.Network.Cliente;
using Microsoft.Extensions.Configuration;
using L2CSharP.Network;
using L2CSharP.Network.Gameserver.Packets.Receive;
using Microsoft.Extensions.Options;
using L2CSharP.Network.Gameserver;
using L2CSharP.LoginServer.L2CSharP.LoginServer;
using L2CSharP.Network.Cliente.Packets;
using L2CSharP.Network.Gameserver.Packets;
using L2CSharP.Network.GameServer;
using L2CSharP.Network.Gameserver.Packets.Receive.L2CSharP.Network.Gameserver.Packets.Receive;

var builder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.SetBasePath(Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "config"))
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                  .AddEnvironmentVariables();
        })
    .ConfigureServices((hostContext, services) =>
    {
        // 1. Configure paths and directories
        var basePath = Directory.GetCurrentDirectory();
        var logDirectory = Path.Combine(basePath, "logs", "login");
        Directory.CreateDirectory(logDirectory);



        services.Configure<NetworkSettings>(hostContext.Configuration.GetSection("NetworkSettings"));
        // 3. Configure custom game logger
        services.AddGameLogger(config =>
        {
            config.EnableConsoleLogging = true;
            config.EnableFileLogging = true;
            config.LogDirectory = logDirectory;
            config.FilePrefix = "login_server";
            config.MaxArchiveFiles = 10;


        });
        // 2. Configure logging providers
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddNLog();
        });


        services.AddSingleton<IGameLogger, GameLogger>();
        services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(provider =>
            provider.GetRequiredService<IGameLogger>());

        // Database
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer("Server=.;Database=L2CSharP.DataBase;User ID=sa;Password=sql2022.;Trusted_Connection=False;Encrypt=False;");
        });

        // Services
        services.AddScoped<IAccountService, AccountService>();

        // Processors
        services.AddSingleton<IPacketProcessor, PacketProcessor>();      
        services.AddSingleton<IGamePacketProcessor, GamePacketProcessor>();
        services.AddSingleton<IMaxConnections, MaxConnections>();

        services.AddSingleton<IGameClientProcessor, GameClientProcessor>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<NetworkSettings>>();
            return new GameClientProcessor(
                provider.GetRequiredService<IPacketProcessor>(),
                provider.GetRequiredService<IGameLogger>(),
                provider.GetRequiredService<IMaxConnections>(),
                settings);
        });

        services.AddSingleton<IGameServerProcessor, GameServerProcessor>();

        // Listeners
        services.AddSingleton<L2CSharP.Network.Cliente.GameClientListener>();
        services.AddSingleton<L2CSharP.Network.Gameserver.GameServerListener>();

        // Plugins
        services.AddSingleton<IAssemblyLoader, DefaultAssemblyLoader>();
        services.AddSingleton<IChronicleProcessor, ChronicleProcessor>(provider =>
            new ChronicleProcessor(
                Path.Combine(Environment.CurrentDirectory, "ChroniclePlugins"),
                provider.GetRequiredService<IGameLogger>(),
                provider.GetRequiredService<IAssemblyLoader>()));

        // Application services
        services.AddSingleton<LoginServer>();
        services.AddHostedService<LoginServerService>();

        // Packets
        services.AddTransient<R_CloseConnection>();
        services.AddTransient<R_CurPlayers>();
        services.AddTransient<R_ServerInfo>();


        // Registrar dependências necessárias
        services.AddSingleton<IServerList, ServerList>();
        services.AddSingleton<IAccountService, AccountService>();


    }).UseConsoleLifetime();



// Configure NLog
LogManager.Setup()
    .LoadConfigurationFromFile(Path.Combine("Config", "nlog.config"))
    .GetCurrentClassLogger();

LogManager.Configuration.Variables["logDirectory"] = Path.Combine("logs", "login");

try
{
    var host = builder.Build();
    var logger = host.Services.GetRequiredService<IGameLogger>();

    // Configure console control handler
    var handler = new ConsoleExitHandler(logger);
    NativeMethods.SetConsoleCtrlHandler(handler.HandleConsoleEvent, true);

    logger.Info("==================================================================");
    logger.Info(" Starting L2CodCraft Login Server");
    logger.Info($" Log Directory: {Path.Combine(Directory.GetCurrentDirectory(), "logs", "login")}");
    logger.Info("==================================================================");

    await host.RunAsync();
}
catch (Exception ex)
{
    LogManager.GetCurrentClassLogger().Fatal(ex, $"Server terminated unexpectedly {ex.ToString()}");
    throw;
}
finally
{
    LogManager.Shutdown();
    Console.WriteLine("Server has been shut down. Press any key to exit...");
    Console.ReadKey();
}

// Console control handler class
internal class ConsoleExitHandler
{
    private readonly IGameLogger _logger;

    public ConsoleExitHandler(IGameLogger logger)
    {
        _logger = logger;
    }

    public bool HandleConsoleEvent(CtrlType sig)
    {
        _logger.LogInformation($"Received shutdown signal: {sig}");
        // Add your shutdown logic here
        Environment.Exit(0);
        return true; // Indicates we handled the event
    }
}

// Control type enumeration
internal enum CtrlType : byte
{
    CTRL_C_EVENT = 0,
    CTRL_BREAK_EVENT = 1,
    CTRL_CLOSE_EVENT = 2,
    CTRL_LOGOFF_EVENT = 5,
    CTRL_SHUTDOWN_EVENT = 6
}

// Native methods interop
internal static class NativeMethods
{
    [DllImport("Kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetConsoleCtrlHandler(
        ConsoleEventDelegate handler,
        [MarshalAs(UnmanagedType.Bool)] bool add);

    public delegate bool ConsoleEventDelegate(CtrlType sig);
}