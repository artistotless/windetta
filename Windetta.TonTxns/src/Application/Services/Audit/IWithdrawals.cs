using Windetta.Common.Types;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.Services.Audit;

public interface IWithdrawals : IScopedService
{
    Task<bool> ExistAsync(Guid id);
    Task AddAsync(Withdrawal transaction);
    Task UpdateAsync(Withdrawal transaction);
}
