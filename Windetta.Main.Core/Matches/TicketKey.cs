using System.Diagnostics.CodeAnalysis;

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

    public override int GetHashCode()
        => HashCode.Combine(matchId, playerId);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not TicketKey t)
            return false;

        return t.matchId.Equals(matchId) && t.playerId.Equals(playerId);
    }
}
