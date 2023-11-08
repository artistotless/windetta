using System.Data;

namespace Windetta.Wallet.Application.DAL;

public interface UnitOfWork
{
    public IWalletsRepository Wallets { get; set; }
    public ITransactionsRepository Transactions { get; set; }

    public Task SaveChangesAsync();
}

public interface UnitOfWorkCommittable : UnitOfWork
{
    public void BeginTransaction(IsolationLevel level);
    public void Commit();
    public void Rollback();
}