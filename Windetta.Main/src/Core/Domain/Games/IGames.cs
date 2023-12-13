using Windetta.Common.Types;

namespace Windetta.Main.Core.Domain.Games;

public interface IGames : IScopedService
{
    public Task AddAsync(Game game);
    public Task RemoveAsync(Game game);
    public Task UpdateAsync(Game game);
    public Task<Game?> GetAsync(Guid id);
    public Task<(GameConfiguration, IEnumerable<SupportedCurrency>)> GetConfigurationsAsync(Guid id);
}