using System.Data;

namespace Windetta.TonTxns.Application.DAL;

public interface UnitOfWork
{
    public ITransactionsRepository Transactions { get; set; }

    public Task SaveChangesAsync();
}

public interface UnitOfWorkCommittable : UnitOfWork
{
    public void BeginTransaction(IsolationLevel level);
    public void Commit();
    public void Rollback();
}