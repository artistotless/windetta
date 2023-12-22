using Windetta.Contracts;
using Windetta.Main.Core.MatchHubs;

namespace Windetta.Main.Core.Matches;

public class Match
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Bet Bet { get; set; }
    public MatchState State { get; set; }
    public IEnumerable<Player> Players { get; set; }
    public IEnumerable<Guid>? Winners { get; set; }
}