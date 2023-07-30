using TonLibDotNet;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.Services;

public class UserWalletService : IUserWalletService
{
    private readonly ILogger _logger;
    //private readonly IWalletsRepository _rep;
    //private readonly IWalletCredentialRepository _rep;

    public UserWalletService(ITonClient tonClient)
    {
        _tonClient = tonClient;
    }

    public Task<WalletBalance> GetBalance(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetDepositAddress(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task HoldBalance(Guid userId, int nanotonAmount)
    {
        throw new NotImplementedException();
    }

    public Task UnHoldBalance(Guid userId)
    {
        throw new NotImplementedException();
    }
}
