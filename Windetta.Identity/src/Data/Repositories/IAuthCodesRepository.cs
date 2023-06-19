using Windetta.Common.Authentication;
using Windetta.Common.Types;

namespace Windetta.Identity.Data.Repositories;

public interface IAuthCodesRepository : IScopedService
{
    Task AddAsync(AuthorizationCode value);
    Task<AuthorizationCode> GetAsync(string key);
    Task RemoveAsync(string key);
}
