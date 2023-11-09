using Windetta.TonTxns.Application.DAL;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.Services.Audit;

public class WithdrawalsService : IWithdrawalsService
{
    private readonly UnitOfWork _uow;

    public WithdrawalsService(UnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> ExistAsync(Guid id)
        => await _uow.Withdrawals.ExistAsync(id);

    public async Task AddAsync(Withdrawal transaction)
    {
        transaction.Created = DateTimeOffset.UtcNow;

        await _uow.Withdrawals.AddAsync(transaction);

        await _uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(Withdrawal transaction)
    {
        transaction.LastModified = DateTimeOffset.UtcNow;

        await _uow.Withdrawals.UpdateAsync(transaction);

        await _uow.SaveChangesAsync();
    }
}
