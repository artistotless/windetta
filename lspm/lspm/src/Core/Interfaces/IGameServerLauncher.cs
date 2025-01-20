using LSPM.Core.Models;

namespace LSPM.Core.Interfaces;

public interface IGameServerLauncher
{
    /// <summary>
    /// Runs the game server of a specific game
    /// </summary>
    /// <param name="gameId">Id of the game for which the game server is started</param>
    /// <returns>Endpoint for connecting players</returns>
    ValueTask<GameServerInfo> LaunchAsync(Guid gameId);
}