namespace Windetta.Main.Core.Domain.MatchHubs.Plugins;

public interface IAutoReadyStrategy : IMatchHubPlugin, IDisposable
{
    void Start(IHubReadyListener hub);
}
