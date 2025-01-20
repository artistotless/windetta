using LSPM.Models;

namespace LSPM.Core.Interfaces;

/// <summary>
/// Service for match management on the game server
/// </summary>
public interface IGameServerMatchClient
{
    /// <summary>
    /// Notifies the running game server of a new match 
    /// it needs to prepare before players connect.
    /// </summary>
    /// <param name="endpoint">Game server endpoint</param>
    /// <param name="initData">Data for match initialization</param>
    Task CreateMatchAsync(Uri endpoint, MatchInitializationData initData);

    /// <summary>
    /// Cancels the match
    /// </summary>
    /// <param name="endpoint">Game server endpoint</param>
    Task CancelMatchAsync(Uri endpoint, Guid matchId);
}