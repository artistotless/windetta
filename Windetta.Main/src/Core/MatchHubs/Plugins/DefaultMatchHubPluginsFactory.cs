using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Core.MatchHubs;

public sealed class DefaultMatchHubPluginsFactory : IMatchHubPluginsFactory
{
    private readonly IEnumerable<IMatchHubPlugin> _plugins;

    public DefaultMatchHubPluginsFactory(IEnumerable<IMatchHubPlugin> plugins)
    {
        _plugins = plugins;
    }

    public IMatchHubPlugin Get(string typeName)
    {
        if (typeName is null)
            throw new ArgumentNullException(nameof(typeName));

        var plugin = _plugins.Where(p => p.GetType().Name == typeName)
            .FirstOrDefault();

        if (plugin is null)
            throw MatchHubException.InvalidPluginId;

        return plugin;
    }

    public T GetOrDefaultImplementation<T>(string? typeName) where T : IMatchHubPlugin
    {
        IMatchHubPlugin? plugin;

        if (typeName is null)
        {
            plugin = _plugins
                .Where(p => p.GetType().Name.Contains("Default"))
                .Where(p => p.GetType().IsAssignableTo(typeof(T)))
                .Where(p => p.GetType().IsAbstract == false)
                .Where(p => p.GetType().IsClass)
                .FirstOrDefault();

            if (plugin is null)
                throw new Exception("Default implementation does not exist");
        }

        plugin = _plugins.Where(p => p.GetType().Name == typeName)
           .FirstOrDefault();

        if (plugin is null)
            throw MatchHubException.InvalidPluginId;

        return (T)plugin;
    }
}