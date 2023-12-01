using Windetta.Common.Types;
using Windetta.Main.Core.MatchHub.Dtos;

namespace Windetta.Main.MatchHub;

public interface IMatchHubs : ISingletonService
{
    public Task AddAsync(IMatchHub hub);
    public Task RemoveAsync(Guid hubId);
    public Task<IEnumerable<MatchHubDto>> GetAllAsync();
    public Task<IMatchHub?> GetAsync(Guid hubId);
    public Task UpdateAsync(IMatchHub hub);
}
