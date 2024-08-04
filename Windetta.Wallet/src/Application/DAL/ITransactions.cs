using Windetta.Common.Types;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.DAL;

public interface ITransactions : IScopedService
{
    public Task AddAsync(Transaction txn);
    public Task<Transaction?> GetAsync(Guid id);
}