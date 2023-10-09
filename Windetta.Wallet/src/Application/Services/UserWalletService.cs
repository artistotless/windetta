using Microsoft.EntityFrameworkCore;
using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Data;
using Windetta.Wallet.Data.Specifications;
using Windetta.Wallet.Domain;
using Windetta.Wallet.Infrastructure.Models;
using Windetta.Wallet.Infrastructure.Services;

namespace Windetta.Wallet.Application.Services;

public class UserWalletService : IUserWalletService
{
    private readonly WalletDbContext _ctx;
    private readonly ITonService _tonService;

    public UserWalletService(WalletDbContext ctx, ITonService tonService)
    {
        _ctx = ctx;
        _tonService = tonService;
    }

    public async Task CreateWalletAsync(Guid userId)
    {
        var walletData = await _tonService.GenerateWalletData();

        var keySet = new WalletKeysSet()
        {
            PrivateKey = walletData.Credential.SecretKey,
            PublicKey = walletData.Credential.PublicKey
        };

        _ctx.Wallets.Add(new UserWallet()
        {
            Address = walletData.Address,
            UserId = userId,
            WalletKeys = keySet
        });

        await _ctx.SaveChangesAsync();
    }

    public async Task<WalletInfoDto> GetWalletInfoAsync(Guid userId)
    {
        var wallet = await _ctx.Wallets.Where(new WalletByUserIdSpec(userId))
              .Select(x => new { x.HeldBalance, x.Address })
              .FirstOrDefaultAsync();

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        var nanotons = await _tonService.GetBalance(wallet.Address);
        var balance = new WalletBalanceDto(nanotons, wallet.HeldBalance);

        return new WalletInfoDto(balance, wallet.Address);
    }

    public async Task HoldBalanceAsync(Guid userId, int nanotons)
    {
        var wallet = await _ctx.Wallets
            .FirstOrDefaultAsync(new WalletByUserIdSpec(userId));

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        wallet.HoldBalance(nanotons);

        await _ctx.SaveChangesAsync();
    }

    public async Task TransferAsync(Guid userId, long nanotons, TonAddress destination)
    {
        var wallet = await _ctx.Wallets.Include(x => x.WalletKeys)
           .FirstOrDefaultAsync(new WalletByUserIdSpec(userId));

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        var credential = new TonWalletCredential()
        {
            PublicKey = wallet.WalletKeys.PublicKey,
            SecretKey = wallet.WalletKeys.PrivateKey
        };

        await _tonService.TransferTon(credential, destination, nanotons);
    }

    public async Task UnHoldBalanceAsync(Guid userId)
    {
        var wallet = await _ctx.Wallets
           .FirstOrDefaultAsync(new WalletByUserIdSpec(userId));

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        wallet.UnHoldBalance();

        await _ctx.SaveChangesAsync();
    }
}
