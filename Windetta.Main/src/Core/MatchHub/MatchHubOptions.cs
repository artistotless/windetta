using Windetta.Main.Games;
using Windetta.Main.MatchHub.Strategies;

namespace Windetta.Main.MatchHub;

public record MatchHubOptions
{
    public Bet Bet { get; init; }
    public GameConfiguration GameConfiguration { get; init; }
    public bool Private { get; init; }
    public AutoReadyStrategy? AutoReadyStrategy { get; init; }
    public AutoDisposeStrategy? AutoDisposeStrategy { get; init; }

    public string[]? JoinFilters { get; init; }
}

public record TournamentMatchHubOptions : MatchHubOptions
{
    public Guid OrganizerId { get; set; }
    public string? Description { get; set; }
    public string? Site { get; set; }
}
