using Windetta.Common.Types;

namespace Windetta.Main.Core.Domain.MatchHubs.Plugins;

public interface IMatchHubPluginsFactory : IScopedService
{
    public T Get<T>(string pluginName, Dictionary<string, string>? requirementsValues) where T : IMatchHubPlugin;
}