using Microsoft.EntityFrameworkCore;
using System.Data;
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
    public Task<WalletBalanceDto> GetBalance(Guid userId)
    {
        throw new NotImplementedException();
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

    public async Task TopUpBalance(TopUpArgument arg)
    {
        if ((await _ctx.Transactions
            .FindAsync(new TxnByIdSpec(arg.OperationId))) is not null) return;

        var wallet = await _ctx.Wallets
           .FindAsync(new WalletByUserIdSpec(arg.userId));

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        using var transaction = _ctx.Database
            .BeginTransaction(IsolationLevel.Serializable);

        try
        {
            wallet.IncreaseBalance(arg.amount);

            _ctx.Transactions.Add(new()
            {
                Id = arg.OperationId,
                Amount = arg.amount,
                TimeStamp = DateTime.UtcNow,
                Type = TransactionType.TopUp,
                UserId = arg.userId
            });

            await _ctx.SaveChangesAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    public async Task TransferAsync(TransferArgument arg)
    {
        if ((await _ctx.Transactions
            .FindAsync(new TxnByIdSpec(arg.OperationId))) is not null) return;

        var user1Wallet = await _ctx.Wallets
            .FindAsync(new WalletByUserIdSpec(arg.userId));

        var user2Wallet = await _ctx.Wallets
            .FindAsync(new WalletByUserIdSpec(arg.destinationUser));

        if (user1Wallet is null || user2Wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        using var transaction = _ctx.Database
            .BeginTransaction(IsolationLevel.Serializable);

        try
        {
            user1Wallet.TransferToWallet(user2Wallet, arg.amount);

            _ctx.Transactions.Add(new()
            {
                Id = arg.OperationId,
                Amount = arg.amount,
                TimeStamp = DateTime.UtcNow,
                Type = TransactionType.Transfer,
                UserId = arg.userId
            });

            await _ctx.SaveChangesAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    public async Task DeductAsync(DeductArgument arg)
    {
        if ((await _ctx.Transactions
           .FindAsync(new TxnByIdSpec(arg.OperationId))) is not null) return;

        var wallet = await _ctx.Wallets
            .FindAsync(new WalletByUserIdSpec(arg.userId));

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        using var transaction = _ctx.Database
            .BeginTransaction(IsolationLevel.Serializable);

        try
        {
            wallet.DecreaseBalance(arg.amount);

            _ctx.Transactions.Add(new()
            {
                Id = arg.OperationId,
                Amount = arg.amount,
                TimeStamp = DateTime.UtcNow,
                Type = TransactionType.Withdrawal,
                UserId = arg.userId
            });

            await _ctx.SaveChangesAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    public async Task CancelDeductAsync(Guid operationId)
    {
        var txn = await _ctx.Transactions
             .FindAsync(new TxnByIdSpec(operationId));

        if (txn is null) return;

        var wallet = await _ctx.Wallets
            .FindAsync(new WalletByUserIdSpec(txn.UserId));

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        using var transaction = _ctx.Database
            .BeginTransaction(IsolationLevel.Serializable);

        try
        {
            wallet.IncreaseBalance(txn.Amount);

            txn.TimeStamp = DateTime.UtcNow;
            txn.Type = TransactionType.CancelWithdrawal;

            await _ctx.SaveChangesAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
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
