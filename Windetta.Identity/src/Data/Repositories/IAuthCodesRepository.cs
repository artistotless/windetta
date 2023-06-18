using Windetta.Common.Types;

namespace Windetta.Identity.Data.Repositories;

public interface IAuthCodesRepository : IScopedService
{
    Task AddAsync();
}
