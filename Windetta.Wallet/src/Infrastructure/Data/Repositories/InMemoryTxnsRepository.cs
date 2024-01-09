using System.Collections.Concurrent;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Infrastructure.Data.Repositories;

public sealed class InMemoryTxnsRepository : ITransactions
{
    private static readonly ConcurrentDictionary<Guid, Transaction> _store;

    static InMemoryTxnsRepository()
    {
        _store = new();
    }

    public Task AddAsync(Transaction txn)
    {
        _store.TryAdd(txn.Id, txn);

        return Task.CompletedTask;
    }

    public Task<Transaction?> GetAsync(Guid id)
    {
        var result = _store
            .TryGetValue(id, out var transaction)
            ? transaction : null;

        return Task.FromResult(result);
    }
}
