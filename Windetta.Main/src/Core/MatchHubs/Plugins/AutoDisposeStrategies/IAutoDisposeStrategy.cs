using Windetta.Main.Core.MatchHubs;

namespace Windetta.Main.MatchHubs.Strategies;

internal interface IAutoDisposeStrategy : IMatchHubPlugin, IDisposable
{
    void Start(IHubDisposeListener hub);
}
