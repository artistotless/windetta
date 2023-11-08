using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.DAL
{
    public interface ITransactionsRepository
    {
        public Task AddAsync(Transaction txn);
        public Task<Transaction?> GetAsync(Guid id);
        public Task<bool> ExistAsync(Guid id);
        Task UpdateAsync(Transaction transaction);
    }
}