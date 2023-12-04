using Windetta.Main.Games;
using Windetta.Main.MatchHubs.Filters;
using Windetta.Main.MatchHubs.Strategies;

namespace Windetta.Main.MatchHubs;

public record MatchHubOptions
{
    public Guid InitiatorId { get; init; }
    public Guid GameId { get; init; }
    public Bet Bet { get; init; }
    public GameConfiguration GameConfiguration { get; init; }
    public bool Private { get; init; }
    public AutoReadyStrategy? AutoReadyStrategy { get; init; }
    public AutoDisposeStrategy? AutoDisposeStrategy { get; init; }
    public IEnumerable<IJoinFilter>? JoinFilters { get; init; }
}

