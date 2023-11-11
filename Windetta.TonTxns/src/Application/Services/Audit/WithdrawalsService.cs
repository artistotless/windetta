using Windetta.TonTxns.Application.DAL;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Application.Services.Audit;

public class WithdrawalsService : IWithdrawalsService
{
    private readonly IUnitOfWork _uow;

    public WithdrawalsService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> ExistAsync(Guid id)
        => await _uow.Withdrawals.ExistAsync(id);

    public async Task AddAsync(Withdrawal withdrawal)
    {
        withdrawal.Created = DateTimeOffset.UtcNow;

        await _uow.Withdrawals.AddAsync(withdrawal);

        await _uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(Withdrawal withdrawal)
    {
        withdrawal.LastModified = DateTimeOffset.UtcNow;

        await _uow.Withdrawals.UpdateAsync(withdrawal);

        await _uow.SaveChangesAsync();
    }
}
