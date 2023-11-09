using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.DAL
{
    public interface IWithdrawalsRepository
    {
        public Task AddAsync(Withdrawal txn);
        public Task<Withdrawal?> GetAsync(Guid id);
        public Task<bool> ExistAsync(Guid id);
        Task UpdateAsync(Withdrawal transaction);
    }
}