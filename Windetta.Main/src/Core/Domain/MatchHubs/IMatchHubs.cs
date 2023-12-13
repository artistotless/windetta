using Windetta.Common.Types;
using Windetta.Main.Core.Domain.MatchHubs.Dtos;

namespace Windetta.Main.Core.Domain.MatchHubs;

public interface IMatchHubs : ISingletonService
{
    public Task AddAsync(IMatchHub hub);
    public Task RemoveAsync(Guid hubId);
    public Task<IEnumerable<MatchHubDto>> GetAllAsync();
    public Task<IMatchHub?> GetAsync(Guid hubId);
    public Task UpdateAsync(IMatchHub hub);
}
