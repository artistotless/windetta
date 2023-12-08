namespace Windetta.Main.Core.Games;

public struct GameConfiguration
{
    public uint MaxPlayers { get; init; }
    public uint MinPlayers { get; init; }
    public uint MaxTeams { get; init; }
    public uint MinTeams { get; init; }

    public GameConfiguration(uint maxPlayers, uint minPlayers) : this()
    {
        MaxPlayers = maxPlayers;
        MinPlayers = minPlayers;
    }
}
