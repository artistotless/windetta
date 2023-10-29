using Microsoft.EntityFrameworkCore;
using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Data;
using Windetta.Wallet.Data.Specifications;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.Services;

public class UserWalletService : IUserWalletService
{
    private readonly WalletDbContext _ctx;

    public UserWalletService(WalletDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<WalletInfoDto> CreateWalletAsync(Guid userId)
    {
        var userWallet = new UserWallet()
        {
            UserId = userId,
        };

        var foundWallet = await _ctx.Wallets.FindAsync(userId);

        if (foundWallet is not null)
            return new WalletInfoDto(
                new(foundWallet.Balance, foundWallet.HeldBalance));

        _ctx.Wallets.Add(userWallet);

        await _ctx.SaveChangesAsync();

        return new WalletInfoDto(new(0, 0));
    }

    public async Task HoldBalanceAsync(Guid userId, long nanotons)
    {
        var wallet = await _ctx.Wallets
            .FirstOrDefaultAsync(new WalletByUserIdSpec(userId));

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        wallet.HoldBalance(nanotons);

        await _ctx.SaveChangesAsync();
    }

    public async Task IncreaseBalance(Guid userId, long nanotons, string txnId)
    {
        if ((await _ctx.Transactions
            .FindAsync(new TxnByIdSpec(txnId))) is not null) return;

        var wallet = await _ctx.Wallets
           .FindAsync(new WalletByUserIdSpec(userId));

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        wallet.IncreaseBalance(nanotons);

        _ctx.Transactions.Add(new()
        {
            Id = txnId,
            Nanotons = nanotons,
            TimeStamp = DateTime.UtcNow,
            Type = TransactionType.TopUp,
            UserId = userId
        });

        await _ctx.SaveChangesAsync();
    }

    public Task TransferAsync(Guid userId, long nanotons, Guid destinationUser)
    {
        throw new NotImplementedException();
    }

    public Task WithdrawAsync(Guid userId, long nanotons, TonAddress destinationAddress)
    {
        throw new NotImplementedException();
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
