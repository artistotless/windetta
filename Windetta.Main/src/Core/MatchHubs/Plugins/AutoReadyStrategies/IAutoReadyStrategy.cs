namespace Windetta.Main.Core.MatchHubs.Plugins;

public interface IAutoReadyStrategy : IMatchHubPlugin, IDisposable
{
    void Start(IHubReadyListener hub);
}
