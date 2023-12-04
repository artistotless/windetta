namespace Windetta.Main.MatchHubs;

public class CreateMatchHubRequest
{
    public Guid InitiatorId { get; init; }
    public Guid GameId { get; init; }
    public Bet Bet { get; init; }
    public bool Private { get; init; }
    public string? AutoReadyStrategy { get; init; }
    public string? AutoDisposeStrategy { get; init; }
    public string[]? JoinFilters { get; init; }
}
