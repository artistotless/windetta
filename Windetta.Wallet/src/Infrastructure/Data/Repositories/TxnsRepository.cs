using Microsoft.EntityFrameworkCore;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Infrastructure.Data.Repositories;

public sealed class TxnsRepository : ITransactions
{
    private readonly WalletDbContext _dbContext;

    public TxnsRepository(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void Add(Transaction txn)
    {
        _dbContext.Transactions.Add(txn);
    }

    public async Task<Transaction?> GetAsync(Guid id)
    {
        var query = _dbContext.Transactions.Where(t => t.Id == id);

        return await query.FirstOrDefaultAsync();
    }
}
