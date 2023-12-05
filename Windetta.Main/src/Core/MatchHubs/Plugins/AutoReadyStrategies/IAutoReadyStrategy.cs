using Windetta.Main.Core.MatchHubs;

namespace Windetta.Main.MatchHubs.Strategies;

internal interface IAutoReadyStrategy : IMatchHubPlugin, IDisposable
{
    void Start(IHubReadyListener hub);
}
