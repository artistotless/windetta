using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Infrastructure.Data.Repositories;

namespace Windetta.Wallet.Infrastructure.Data;

public class DbUnitOfWork : UnitOfWork
{
    public Lazy<IWallets> Wallets { get; init; }
    public Lazy<ITransactions> Transactions { get; init; }

    private WalletDbContext _dbContext;

    public DbUnitOfWork(WalletDbContext dbContext)
    {
        _dbContext = dbContext;

        Wallets = new Lazy<IWallets>(() => new WalletsRepository(dbContext));
        Transactions = new Lazy<ITransactions>(() => new TxnsRepository(dbContext));
    }

    public IDbTransaction BeginTransaction(IsolationLevel level)
    {
        return _dbContext.Database.BeginTransaction(level).GetDbTransaction();
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
