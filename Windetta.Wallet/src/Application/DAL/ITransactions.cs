using Windetta.Common.Types;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.DAL;

public interface ITransactions : IScopedService
{
    public void Add(Transaction txn);
    public Task<Transaction?> GetAsync(Guid id);
}