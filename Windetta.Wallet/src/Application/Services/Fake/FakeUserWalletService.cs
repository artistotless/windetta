using Windetta.Common.Types;
using Windetta.Contracts;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.Services.Fake;

public class FakeUserWalletService : IUserWalletService
{
    public Task CancelDeductAsync(Guid operationId)
    {
        return Task.CompletedTask;
    }

    public Task CreateWalletAsync(Guid userId, IEnumerable<UserBalance>? initial = null)
    {
        return Task.CompletedTask;
    }

    public Task DeductAsync(DeductArgument arg)
    {
        return Task.CompletedTask;
    }

    public Task DeductUnHoldAsync(IEnumerable<Guid> userIds, FundsInfo funds)
    {
        throw new NotImplementedException();
    }

    public Task DeductUnHoldAsync(DeductUnHoldArgument arg)
    {
        throw new NotImplementedException();
    }

    public Task<UserBalance> GetBalance(Guid userId, int currencyId)
    {
        throw new NotImplementedException();
    }

    public Task HoldBalanceAsync(Guid userId, FundsInfo funds)
    {
        return Task.CompletedTask;
    }

    public Task HoldBalanceAsync(IEnumerable<Guid> userIds, FundsInfo funds)
    {
        return Task.CompletedTask;
    }

    public Task IncreaseBalance(IncreaseArgument arg)
    {
        return Task.CompletedTask;
    }

    public Task TransferAsync(TransferArgument arg)
    {
        return Task.CompletedTask;
    }

    public Task UnHoldBalanceAsync(Guid userId, FundsInfo funds)
    {
        return Task.CompletedTask;
    }

    public Task UnHoldBalanceAsync(IEnumerable<Guid> userIds, FundsInfo funds)
    {
        return Task.CompletedTask;
    }
}
