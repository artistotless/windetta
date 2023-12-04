using Windetta.Common.Types;

namespace Windetta.Main.Core.MatchHubs;

public interface IMatchHubPluginsFactory : IScopedService
{
    public IMatchHubPlugin Get(string typeName);
    public T GetOrDefaultImplementation<T>(string? typeName) where T : IMatchHubPlugin;
}