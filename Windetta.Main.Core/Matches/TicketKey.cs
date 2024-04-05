namespace Windetta.Main.Core.Matches;

/// <summary>
/// Represents a complex key to get a player's ticket
/// </summary>
public readonly struct TicketKey
{
    public readonly Guid matchId;
    public readonly Guid playerId;

    public TicketKey(Guid matchId, Guid playerId)
    {
        this.matchId = matchId;
        this.playerId = playerId;
    }
}
