namespace Windetta.Main.Core.Matches;

/// <summary>
/// The structure of the match that is going on right now
/// </summary>
public struct OngoingMatch
{
    /// <summary>
    /// Match ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Ticket for player identification when connecting to the game server
    /// </summary>
    public string Ticket { get; init; }

    public OngoingMatch(Guid matchId, string ticket)
    {
        Id = matchId;
        Ticket = ticket;
    }
}
