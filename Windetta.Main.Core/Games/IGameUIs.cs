using Windetta.Common.Types;

namespace Windetta.Main.Core.Games;

/// <summary>
/// Service storing game ui contents
/// </summary>
public interface IGameUIs : IScopedService
{
    /// <summary>
    /// Adds the game UI
    /// </summary>
    public Task AddAsync(Guid gameId, string uiContent);

    /// <summary>
    /// Removes the game UI
    /// </summary>
    public Task RemoveAsync(Guid gameId);

    /// <summary>
    /// Updates the game UI
    /// </summary>
    public Task UpdateAsync(Guid gameId, string uiContent);

    /// <summary>
    /// Gets the game UI
    /// </summary>
    /// <returns>HTML+Scripts</returns>
    public Task<string> GetAsync(Guid gameId);
}