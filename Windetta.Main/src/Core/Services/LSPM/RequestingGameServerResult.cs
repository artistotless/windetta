namespace Windetta.Main.Core.Services.LSPM;

public class RequestingGameServerResult
{
    /// <summary>
    /// If LSPM can accept the request and prepare the game server, 
    /// the result will be 'success = true'. 
    /// If LSPM cannot accept the request and prepare the game server, 
    /// the result will be 'success = false'.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// If LSPM has an unloaded game server and is ready to provide information to connect to it immediately, 
    /// IsCompletedResponse will be set to true, and "Info" will always contain data.
    /// If LSPM does not have a game server ready, 
    /// it will need to spend some time starting and preparing it, 
    /// including port configuration.In this case, IsCompletedResponse will be set to false, 
    /// and "Info" will be delivered through a separate event called IReservedGameServerPrepared.
    /// </summary>
    public bool IsCompletedResponse { get; set; }
    public GameServerInfo? Info { get; set; }
    public string? Error { get; set; }
}