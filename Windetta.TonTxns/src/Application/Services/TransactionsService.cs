using Microsoft.EntityFrameworkCore;
using Windetta.TonTxns.Domain;
using Windetta.TonTxns.Infrastructure.Data;

namespace Windetta.TonTxns.Application.Services;

public class TransactionsService : ITransactionsService
{
    private readonly TonDbContext _ctx;

    public TransactionsService(TonDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<bool> ExistAsync(Guid id)
        => await _ctx.Transactions.AnyAsync(x => x.Id == id);

    public async Task AddAsync(Transaction transaction)
    {
        transaction.Created = DateTimeOffset.UtcNow;

        _ctx.Transactions.Add(transaction);

        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Transaction transaction)
    {
        transaction.LastModified = DateTimeOffset.UtcNow;

        _ctx.Transactions.Update(transaction);

        await _ctx.SaveChangesAsync();
    }
}
