using Windetta.Common.Types;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.Services;

public interface ITransactionsService : IScopedService
{
    Task<bool> ExistAsync(Guid id);
    Task AddAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction);
}
