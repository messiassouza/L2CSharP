using L2CSharP.LoggerApi.Logger.Interfaces;
using L2CSharP.Network.Cliente.Interfaces;
using L2CSharP.Network.Cliente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace L2CSharP.LoginServer
{
    public class ChronicleProcessor : IChronicleProcessor
    {
        private readonly List<ChroniclePlugin> _plugins;
        private readonly IGameLogger _logger;
        private readonly IAssemblyLoader _assemblyLoader;

        public string PluginDirectory { get; }

        public ChronicleProcessor(
            string pluginDirectory,
            IGameLogger logger,
            IAssemblyLoader assemblyLoader)
        {
            PluginDirectory = pluginDirectory ?? throw new ArgumentNullException(nameof(pluginDirectory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _assemblyLoader = assemblyLoader ?? throw new ArgumentNullException(nameof(assemblyLoader));
            _plugins = new List<ChroniclePlugin>();
        }

        public void Initialize()
        {
            try
            {
                EnsurePluginDirectoryExists();
                LoadPlugins();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize ChronicleProcessor: {ex.Message}");
                throw;
            }
        }

        private void EnsurePluginDirectoryExists()
        {
            if (!Directory.Exists(PluginDirectory))
            {
                _logger.LogInformation($"Creating plugin directory: {PluginDirectory}");
                Directory.CreateDirectory(PluginDirectory);
            }
        }

        private void LoadPlugins()
        {
            foreach (string file in Directory.GetFiles(PluginDirectory, "*.dll"))
            {
                try
                {
                    LoadPluginAssembly(file);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to load plugin from {file}: {ex.Message}");
                }
            }
        }

        private void LoadPluginAssembly(string filePath)
        {
            var assembly = _assemblyLoader.LoadFrom(filePath);
            var pluginTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ChroniclePlugin)));

            foreach (var type in pluginTypes)
            {
                try
                {
                    var plugin = (ChroniclePlugin)Activator.CreateInstance(type);
                    InvokeEntryPoint(plugin);
                    _plugins.Add(plugin);
                    _logger.LogInformation($"Loaded plugin: {type.FullName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to initialize plugin {type.Name}: {ex.Message}");
                }
            }
        }

        private void InvokeEntryPoint(ChroniclePlugin plugin)
        {
            var entryMethod = plugin.GetType().GetMethod("EntryPoint");
            if (entryMethod == null)
            {
                throw new InvalidOperationException($"Plugin {plugin.GetType().Name} is missing EntryPoint method");
            }
            entryMethod.Invoke(plugin, Array.Empty<object>());
        }

        public void OnClientConnect(GameClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            foreach (var plugin in _plugins)
            {
                try
                {
                    plugin.OnClientConnect(client);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Plugin {plugin.GetType().Name} failed in OnClientConnect: {ex.Message}");
                }
            }
        }
    }

    // Interface para abstrair o carregamento de assemblies (melhora a testabilidade)
    public interface IAssemblyLoader
    {
        Assembly LoadFrom(string assemblyPath);
    }

    // Implementação concreta do carregador de assemblies
    public class DefaultAssemblyLoader : IAssemblyLoader
    {
        public Assembly LoadFrom(string assemblyPath) => Assembly.LoadFrom(assemblyPath);
    }
}
