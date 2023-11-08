using System.Data;
using Windetta.Wallet.Application.DAL;

namespace Windetta.Wallet.Infrastructure.Data;

public class DbUnitOfWork : UnitOfWorkCommittable
{
    public IWalletsRepository Wallets { get; set; }
    public ITransactionsRepository Transactions { get; set; }

    public void BeginTransaction(IsolationLevel level)
    {
        throw new NotImplementedException();
    } 

    public void Commit()
    {
        throw new NotImplementedException();
    }

    public void Rollback()
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}
