using Windetta.Common.Types;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.DAL;

[AutoInjectExclude]
public class InMemoryWithdrawalsRepository : IWithdrawalsRepository
{
    private readonly List<Withdrawal> _storage;

    public InMemoryWithdrawalsRepository()
        => _storage = new List<Withdrawal>();

    public Task AddAsync(Withdrawal txn)
    {
        _storage.Add(txn);

        return Task.CompletedTask;
    }

    public Task<bool> ExistAsync(Guid id)
        => Task.FromResult(_storage.Exists(x => x.Id == id));

    public Task<Withdrawal?> GetAsync(Guid id)
        => Task.FromResult(_storage.FirstOrDefault(x => x.Id == id));

    public Task UpdateAsync(Withdrawal transaction)
    {
        var found = _storage.FirstOrDefault(x => x.Id == transaction.Id);

        if (found is null)
            return Task.CompletedTask;

        found.Status = transaction.Status;
        found.Created = transaction.Created;
        found.LastModified = transaction.LastModified;
        found.TotalAmount = transaction.TotalAmount;
        found.TransfersCount = transaction.TransfersCount;

        return Task.CompletedTask;
    }
}