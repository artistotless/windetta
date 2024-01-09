using System.Data;
using Windetta.Wallet.Application.DAL;

namespace Windetta.Wallet.Infrastructure.Data;

public class DbUnitOfWork : UnitOfWorkCommittable
{
    public IWallets Wallets { get; set; }
    public ITransactions Transactions { get; set; }

    public DbUnitOfWork(IWallets wallets, ITransactions transactions)
    {
        Wallets = wallets;
        Transactions = transactions;
    }

    public void BeginTransaction(IsolationLevel level)
    {
        // TODO: implement it
    }

    public void Commit()
    {
        // TODO: implement it
    }

    public void Rollback()
    {
        // TODO: implement it
    }

    public Task SaveChangesAsync()
    {
        // TODO: implement it

        return Task.CompletedTask;
    }
}
