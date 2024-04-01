using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.Lobbies.Plugins;

public sealed class DefaultLobbyPluginsFactory : ILobbyPluginsFactory
{
    private readonly IEnumerable<ILobbyPlugin> _plugins;

    public DefaultLobbyPluginsFactory(IEnumerable<ILobbyPlugin> plugins)
    {
        _plugins = plugins;
    }

    public T Get<T>(string pluginName, Dictionary<string, string>? requirementsValues)
        where T : ILobbyPlugin
    {
        ILobbyPlugin? plugin;

        if (pluginName is null)
            throw new ArgumentNullException(nameof(pluginName));

        plugin = FindPluginsByType<T>()
           .Where(p => p.GetType().Name == pluginName)
           .FirstOrDefault();

        if (plugin is null)
            throw LobbyPluginException.InvalidPluginName;

        if (plugin is ConfigurableLobbyPlugin configurable)
        {
            if (requirementsValues is null)
                throw LobbyPluginException.RequiredValuesNotProvided;

            return (T)(configurable.WithValues(requirementsValues) as ILobbyPlugin);
        }

        return (T)plugin;
    }

    private IEnumerable<ILobbyPlugin> FindPluginsByType<T>()
    {
        return _plugins
              .Where(p => p.GetType().IsAssignableTo(typeof(T)))
              .Where(p => p.GetType().IsAbstract == false)
              .Where(p => p.GetType().IsClass);
    }
}