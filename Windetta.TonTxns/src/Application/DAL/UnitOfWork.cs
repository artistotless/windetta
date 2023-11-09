using System.Data;

namespace Windetta.TonTxns.Application.DAL;

public interface UnitOfWork
{
    public IWithdrawalsRepository Withdrawals { get; set; }

    public Task SaveChangesAsync();
}

public interface UnitOfWorkCommittable : UnitOfWork
{
    public void BeginTransaction(IsolationLevel level);
    public void Commit();
    public void Rollback();
}