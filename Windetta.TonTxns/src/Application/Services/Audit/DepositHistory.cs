using Windetta.TonTxns.Application.DAL;

namespace Windetta.TonTxns.Application.Services.Audit;

public class DepositHistory : IDepositsHistory
{
    private readonly IUnitOfWork _uow;

    public DepositHistory(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> GetLastIdAsync()
        => await _uow.Deposits.GetLastId();

    public async Task UpdateLasIdAsync(Guid lastId)
    {
        await _uow.Deposits.UpdateLastId(lastId);
        await _uow.SaveChangesAsync();
    }
}