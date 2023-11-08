using Windetta.TonTxns.Application.DAL;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.Services;

public class TransactionsService : ITransactionsService
{
    private readonly UnitOfWork _uow;

    public TransactionsService(UnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> ExistAsync(Guid id)
        => await _uow.Transactions.ExistAsync(id);

    public async Task AddAsync(Transaction transaction)
    {
        transaction.Created = DateTimeOffset.UtcNow;

        await _uow.Transactions.AddAsync(transaction);

        await _uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        transaction.LastModified = DateTimeOffset.UtcNow;

        await _uow.Transactions.UpdateAsync(transaction);

        await _uow.SaveChangesAsync();
    }
}
