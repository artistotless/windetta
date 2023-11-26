using Windetta.Common.Types;

namespace Windetta.Main.MatchHub;

public interface IMatchHubs : ISingletonService
{
    public Task AddAsync(IMatchHub hub);
    public Task RemoveAsync(IMatchHub hub);
    public Task<IEnumerable<IMatchHub>> GetAllAsync();
    public Task<IMatchHub> GetAsync(Guid hubId);
}
