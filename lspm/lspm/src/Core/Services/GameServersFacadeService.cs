using LSPM.Core.Exceptions;
using LSPM.Core.Interfaces;
using LSPM.Core.Models;
using LSPM.Infrastructure.Services;
using LSPM.Models;
using Serilog;

namespace LSPM.Core.Services;

public sealed class GameServersFacadeService
{
    /// <summary>
    /// To run game servers
    /// </summary>
    /// 
    private readonly IGameServerLauncher _launcher;

    /// <summary>
    /// To create matches on game servers
    /// </summary>
    private readonly IGameServerMatchClient _matchClient;

    /// <summary>
    /// To store information about running game servers
    /// </summary>
    private readonly IGameServersStore _servers;

    public GameServersFacadeService(
        IGameServerLauncher launcher,
        IGameServerMatchClient matches,
        IGameServersStore servers)
    {
        _launcher = launcher;
        _matchClient = matches;
        _servers = servers;
    }

    /// <summary>
    /// Creates a match on the selected game server
    /// </summary>
    /// <param name="instanceId">Game server identifer</param>
    /// <param name="initData">Data about the match to be created</param>
    public async Task CreateMatchAsync(Guid instanceId, MatchInitializationData initData)
    {
        var serverEntry = _servers.Find(instanceId);

        if (serverEntry is null)
            throw LspmException.GameServerNotFound;

        // The server is not ready to accept a match creation request at this time
        // Defer the request until the server indicates that it is ready to accept requests from the LSPM
        if (!serverEntry.IsReady)
        {
            lock (serverEntry.GetLock())
            {
                if (!serverEntry.IsReady)
                {
                    DelayedMatchesQueue.AddDelayedMatch(serverEntry.InstanceId, initData);
                    return;
                }
            }
        }

        Log.ForContext<GameServersFacadeService>().Information(
    "Notifying the game server {serverInstaceId} about created match...", instanceId);

        await _matchClient.CreateMatchAsync(serverEntry.IpcEndpoint, initData);

        serverEntry.AddMatchWithLock(initData.MatchId);
    }

    /// <summary>
    /// Cancel a match on a specific game server
    /// </summary>
    /// <param name="instanceId">Game server identifier</param>
    /// <param name="matchId">Match identifier</param>
    public async Task CancelMatchAsync(Guid instanceId, Guid matchId)
    {
        var serverEntry = _servers.Find(instanceId);

        if (serverEntry is null)
            throw LspmException.GameServerNotFound;

        Log.ForContext<GameServersFacadeService>().Information(
    "Notifying the game server {serverInstaceId} about canceled match {matchId}...", instanceId, matchId);

        await _matchClient.CancelMatchAsync(serverEntry.IpcEndpoint, matchId);

        serverEntry.RemoveMatchWithLock(matchId);
    }

    /// <summary>
    /// Starts a game server instance
    /// </summary>
    /// <param name="gameId">Game ID/param>
    /// <returns>Information about the running game server</returns>
    public async Task<GameServerInfo> LaunchAsync(Guid gameId)
    {
        var launchResult = await _launcher.LaunchAsync(gameId);

        _servers.Add(new GameServerEntry()
        {
            InstanceId = launchResult.InstanceId,
            Endpoint = launchResult.Endpoint,
            IpcEndpoint = launchResult.IpcEndpoint,
            GameId = gameId,
        });

        return launchResult;
    }

    /// <summary>
    /// Get an unfilled game server from the list of game servers
    /// </summary>
    /// <param name="gameId">Game ID</param>
    /// <returns>Information about the running game server</returns>
    public GameServerEntry? GetLoadFreeServer(Guid gameId)
        => _servers.GetLoadFreeServer(gameId);

    /// <summary>
    /// Check if there is an unfilled game server in the list of game servers
    /// </summary>
    /// <param name="gameId">Game ID</param>
    public bool AnyLoadFreeServer(Guid gameId)
        => _servers.AnyLoadFreeServer(gameId);
}