using Windetta.Main.Core.Games;
using Windetta.Main.Core.MatchHubs.Plugins;

namespace Windetta.Main.Core.MatchHubs;

public record MatchHubOptions
{
    public Guid InitiatorId { get; init; }
    public Guid GameId { get; init; }
    public Bet Bet { get; init; }
    public GameConfiguration GameConfiguration { get; init; }
    public bool Private { get; init; }
    internal IAutoReadyStrategy? AutoReadyStrategy { get; init; }
    internal IAutoDisposeStrategy? AutoDisposeStrategy { get; init; }
    internal IEnumerable<IJoinFilter>? JoinFilters { get; init; }
}

