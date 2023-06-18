using Windetta.Common.Types;

namespace Windetta.Identity.Data.Repositories;

public interface IRefreshTokensRepository : IScopedService
{
    Task AddAsync();
}
