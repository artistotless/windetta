using System.Data;
using Windetta.Common.Types;

namespace Windetta.TonTxns.Application.DAL;

[ExcludeFromAutoInject]
public partial class InMemoryUnitOfWork : IUnitOfWork
{
    public IWithdrawalsRepository Withdrawals { get; set; }
    public IDepositsRepository Deposits { get; set; }

    public InMemoryUnitOfWork(
        IWithdrawalsRepository withdrawals,
        IDepositsRepository deposits)
    {
        Withdrawals = withdrawals;
        Deposits = deposits;
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}