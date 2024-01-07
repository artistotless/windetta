namespace Windetta.Main.Core.Games;

public class GameConfiguration
{
    public uint MaxPlayers { get; init; }
    public uint MinPlayers { get; init; }
    public uint MaxTeams { get; init; }
    public uint MinTeams { get; init; }

    public GameConfiguration()
    {
        
    }

    public GameConfiguration(uint maxPlayers, uint minPlayers)
    {
        MaxPlayers = maxPlayers;
        MinPlayers = minPlayers;
    }
}
