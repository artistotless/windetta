using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.DAL
{
    public interface ITransactionsRepository
    {
        public Task AddAsync(Transaction txn);
        public Task<Transaction?> GetAsync(Guid id);
    }
}