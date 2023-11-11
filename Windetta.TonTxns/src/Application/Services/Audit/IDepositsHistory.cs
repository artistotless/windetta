using Windetta.Common.Types;

namespace Windetta.TonTxns.Application.Services.Audit;

public interface IDepositsHistory : IScopedService
{
    Task<Guid> GetLastIdAsync();
    Task UpdateLasIdAsync(Guid lastId);
}
