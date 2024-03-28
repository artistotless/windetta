using Windetta.Common.Types;

namespace Windetta.Main.Core.Games;

/// <summary>
/// Service storing reference information about games
/// </summary>
public interface IGames : IScopedService
{
    /// <summary>
    /// Adds a new game to the directory
    /// </summary>
    public Task AddAsync(Game game);

    /// <summary>
    /// Deletes the game from the directory
    /// </summary>
    public Task RemoveAsync(Game game);

    /// <summary>
    /// Updates the game
    /// </summary>
    public Task UpdateAsync(Game game);

    /// <summary>
    /// Gets the game by its identifier
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns></returns>
    public Task<Game?> GetAsync(Guid id);

    /// <summary>
    /// Gets the game configuration and supported currencies
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns>Game configuration and supported currencies</returns>
    public Task<(GameConfiguration, IEnumerable<SupportedCurrency>)> GetConfigurationsAsync(Guid id);
}