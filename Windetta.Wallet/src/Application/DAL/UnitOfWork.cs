using System.Data;

namespace Windetta.Wallet.Application.DAL;

public interface UnitOfWork
{
    public Lazy<IWallets> Wallets { get; init; }
    public Lazy<ITransactions> Transactions { get; init; }
    public Task SaveChangesAsync();
    public IDbTransaction BeginTransaction(IsolationLevel level);
}