using Windetta.Common.Types;

namespace Windetta.Main.Core.Services.LSPM;

public interface ILspms
{
    Task<Lspm?> GetAsync(Guid gameId);
    Task<IEnumerable<Lspm>> GetAllAsync();
    Task<IEnumerable<Lspm>> GetAllAsync(Guid gameId);
}
