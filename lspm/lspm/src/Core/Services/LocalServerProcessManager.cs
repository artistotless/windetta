using LSPM.Core.Interfaces;
using LSPM.Core.Models;

namespace LSPM.Core.Services;

public class LocalServerProcessManager : ILocalServerProcessManager
{
    private readonly GameServersFacadeService _gameServers;

    public LocalServerProcessManager(GameServersFacadeService gameServers)
    {
        _gameServers = gameServers;
    }

    /// <summary>
    /// Get an existing game server or start a new one
    /// </summary>
    /// <param name="gameId">Game server for what game</param>
    /// <param name="serverReadyCallback">Game server for what game</param>
    /// <returns>Information about the game server</returns>
    /// <exception cref="ArgumentException">If gameId is empty</exception>
    public async Task<GameServerInfo> GetOrLaunchGameServer(Guid gameId)
    {
        if (gameId == Guid.Empty)
            throw new ArgumentException(nameof(gameId));

        var gameServer = _gameServers.GetLoadFreeServer(gameId);

        return gameServer ?? await _gameServers.LaunchAsync(gameId);
    }
}