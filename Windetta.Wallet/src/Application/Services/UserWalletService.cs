using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Infrastructure.Models;
using Windetta.Wallet.Infrastructure.Services;

namespace Windetta.Wallet.Application.Services;

public class UserWalletService : IUserWalletService
{
    private readonly ITonService _tonService;
    private readonly ILogger _logger;
    //private readonly IWalletsRepository _rep;
    //private readonly IWalletCredentialRepository _rep;

    public UserWalletService(ITonService tonService, ILogger logger)
    {
        _tonService = tonService;
        _logger = logger;
    }

    public Task CreateWallet(Guid userId, TonWallet wallet)
    {
        throw new NotImplementedException();
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
