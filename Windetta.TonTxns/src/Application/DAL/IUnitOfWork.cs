using System.Data;
using Windetta.Common.Types;

namespace Windetta.TonTxns.Application.DAL;

public interface IUnitOfWork : IScopedService
{
    public IWithdrawalsRepository Withdrawals { get; set; }
    public IDepositsRepository Deposits { get; set; }

    public Task SaveChangesAsync();
}

public interface UnitOfWorkCommittable : IUnitOfWork
{
    public void BeginTransaction(IsolationLevel level);
    public void Commit();
    public void Rollback();
}
