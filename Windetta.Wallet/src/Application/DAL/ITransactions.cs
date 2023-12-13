using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.DAL
{
    public interface ITransactions
    {
        public Task AddAsync(Transaction txn);
        public Task<Transaction?> GetAsync(Guid id);
    }
}