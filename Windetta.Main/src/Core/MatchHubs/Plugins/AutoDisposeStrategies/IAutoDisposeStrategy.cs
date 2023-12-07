namespace Windetta.Main.Core.MatchHubs.Plugins;

public interface IAutoDisposeStrategy : IMatchHubPlugin, IDisposable
{
    void Start(IHubDisposeListener hub);
}
