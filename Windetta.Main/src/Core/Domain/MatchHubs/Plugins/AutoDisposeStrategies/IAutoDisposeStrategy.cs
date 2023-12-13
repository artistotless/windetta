namespace Windetta.Main.Core.Domain.MatchHubs.Plugins;

public interface IAutoDisposeStrategy : IMatchHubPlugin, IDisposable
{
    void Start(IHubDisposeListener hub);
}
