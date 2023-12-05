using Windetta.Common.Types;

namespace Windetta.Main.Core.MatchHubs;

public interface IMatchHubPluginsFactory : IScopedService
{
    public T Get<T>(string pluginName, Dictionary<string, string>? requirementsValues) where T : IMatchHubPlugin;
    public T GetOrDefaultImplementation<T>(string? pluginName) where T : IMatchHubPlugin;
}