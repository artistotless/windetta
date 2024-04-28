namespace Windetta.Main.Core.Games;

/// <summary>
/// Configuration of the game
/// </summary>
public class GameConfiguration
{
    /// <summary>
    /// Maximum number of players in a team
    /// </summary>
    public uint MaxPlayersInTeam { get; init; }

    /// <summary>
    /// Minimum number of players in a team
    /// </summary>
    public uint MinPlayersInTeam { get; init; }

    /// <summary>
    /// Maximum number of teams that can play a game in one match
    /// </summary>
    public uint MaxTeams { get; init; }

    /// <summary>
    /// Minumum number of teams that can play a game in one match
    /// </summary>
    public uint MinTeams { get; init; }

    public GameConfiguration()
    {

    }

    public GameConfiguration(uint maxPlayers, uint minPlayers)
    {
        MaxPlayersInTeam = maxPlayers;
        MinPlayersInTeam = minPlayers;
    }
}
