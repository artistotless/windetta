using Windetta.Common.Types;

namespace Windetta.Identity.Data.Repositories;

public interface IRefreshTokensRepository : IScopedService
{
    Task SetAsync(Guid userId, string refreshToken);
    Task<string?> GetAsync(Guid userId);
    Task RemoveAsync(Guid userId);
}
