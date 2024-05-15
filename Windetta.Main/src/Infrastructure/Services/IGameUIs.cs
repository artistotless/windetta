using Windetta.Common.Types;
using Windetta.Main.Infrastructure.GameUIs;

namespace Windetta.Main.Infrastructure.Services;

/// <summary>
/// Service storing game ui contents
/// </summary>
public interface IGameUIs : IScopedService
{
    /// <summary>
    /// Adds the game UI
    /// </summary>
    public Task AddAsync(Guid gameId, GameUIResult uiContent);

    /// <summary>
    /// Removes the game UI
    /// </summary>
    public Task RemoveAsync(Guid gameId);

    /// <summary>
    /// Updates the game UI
    /// </summary>
    public Task UpdateAsync(Guid gameId, GameUIResult uiContent);

    /// <summary>
    /// Gets the game UI
    /// </summary>
    /// <returns>HTML+Scripts</returns>
    public Task<GameUIResult> GetAsync(Guid gameId);
}