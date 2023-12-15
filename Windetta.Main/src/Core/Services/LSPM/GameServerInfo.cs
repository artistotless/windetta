namespace Windetta.Main.Core.Services.LSPM;

public class GameServerInfo
{
    public string? Endpoint { get; set; }
    public Dictionary<Guid, string>? Tickets { get; set; }
}