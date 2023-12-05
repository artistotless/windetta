using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.MatchHubs.Plugins;

namespace Windetta.Main.Core.MatchHubs;

public sealed class DefaultMatchHubPluginsFactory : IMatchHubPluginsFactory
{
    private readonly IEnumerable<IMatchHubPlugin> _plugins;

    public DefaultMatchHubPluginsFactory(IEnumerable<IMatchHubPlugin> plugins)
    {
        _plugins = plugins;
    }

    public T Get<T>(string pluginName,
        Dictionary<string, string>? requirementsValues)
        where T : IMatchHubPlugin
    {
        IMatchHubPlugin? plugin;

        if (pluginName is null)
            throw new ArgumentNullException(nameof(pluginName));

        plugin = FindPluginsByType<T>()
           .Where(p => p.GetType().Name == pluginName)
           .FirstOrDefault();

        if (plugin is null)
            throw MatchHubPluginException.InvalidPluginName;

        if (plugin is ConfigurableMatchHubPlugin configurable)
        {
            if (requirementsValues is null)
                throw MatchHubPluginException.RequiredValuesNotProvided;

            return (T)(configurable.WithValues(requirementsValues) as IMatchHubPlugin);
        }

        return (T)plugin;
    }

    public T GetOrDefaultImplementation<T>(string? pluginName) where T : IMatchHubPlugin
    {
        IMatchHubPlugin? plugin;

        var plugins = FindPluginsByType<T>();

        if (pluginName is null)
        {
            var defaultPlugin = plugins
                .FirstOrDefault(x => x.GetType().Name.Contains("Default"));

            if (defaultPlugin is null)
                throw new Exception("Default implementation does not exist");
        }

        plugin = _plugins.Where(p => p.GetType().Name == pluginName)
           .FirstOrDefault();

        if (plugin is null)
            throw MatchHubPluginException.InvalidPluginName;

        return (T)plugin;
    }

    private IEnumerable<IMatchHubPlugin> FindPluginsByType<T>()
    {
        return _plugins
              .Where(p => p.GetType().IsAssignableTo(typeof(T)))
              .Where(p => p.GetType().IsAbstract == false)
              .Where(p => p.GetType().IsClass);
    }
}