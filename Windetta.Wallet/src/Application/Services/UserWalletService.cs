using System.Data;
using Windetta.Common.Constants;
using Windetta.Common.Types;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application.Services;

public class UserWalletService : IUserWalletService
{
    private readonly UnitOfWorkCommittable _uow;

    public UserWalletService(UnitOfWorkCommittable ctx)
    {
        _uow = ctx;
    }
    public Task<UserBalance> GetBalance(Guid userId, int currencyId)
    {
        throw new NotImplementedException();
    }

    public async Task CreateWalletAsync(Guid userId, IEnumerable<UserBalance>? initial = null)
    {
        var userWallet = new UserWallet()
        {
            UserId = userId,
            Balances = initial?.ToList()
        };

        var foundWallet = await _uow.Wallets.GetAsync(userId);

        if (foundWallet is not null)
            return;

        await _uow.Wallets.AddAsync(userWallet);

        await _uow.SaveChangesAsync();
    }

    public async Task HoldBalanceAsync(Guid userId, int currencyId, ulong amount)
    {
        var wallet = await _uow.Wallets.GetAsync(userId);

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        var balance = wallet.GetBalance(currencyId);

        balance.Hold(amount);

        await _uow.SaveChangesAsync();
    }

    public async Task TopUpBalance(TopUpArgument arg)
    {
        if (await _uow.Transactions
            .GetAsync(arg.OperationId) is not null) return;

        var wallet = await _uow.Wallets.GetAsync(arg.userId);

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        var balance = wallet.GetBalance(arg.currencyId);

        //using var transaction = _ctx.Database
        //    .BeginTransaction(IsolationLevel.Serializable);

        _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            balance.Increase(arg.amount);

            await _uow.Transactions.AddAsync(new()
            {
                Id = arg.OperationId,
                Amount = arg.amount,
                TimeStamp = DateTime.UtcNow,
                Type = TransactionType.TopUp,
                UserId = arg.userId
            });

            await _uow.SaveChangesAsync();

            _uow.Commit();
        }
        catch
        {
            _uow.Rollback();
        }
    }

    public async Task TransferAsync(TransferArgument arg)
    {
        if ((await _uow.Transactions
            .GetAsync(arg.OperationId)) is not null) return;

        var user1Wallet = await _uow.Wallets.GetAsync(arg.userId);
        var user2Wallet = await _uow.Wallets.GetAsync(arg.destinationUserId);

        if (user1Wallet is null || user2Wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            user1Wallet.TransferToWallet(user2Wallet, arg.currencyId, arg.amount);

            await _uow.Transactions.AddAsync(new()
            {
                Id = arg.OperationId,
                Amount = arg.amount,
                TimeStamp = DateTime.UtcNow,
                Type = TransactionType.Transfer,
                UserId = arg.userId
            });

            await _uow.SaveChangesAsync();

            _uow.Commit();
        }
        catch
        {
            _uow.Rollback();
        }
    }

    public async Task DeductAsync(DeductArgument arg)
    {
        if (await _uow.Transactions
           .GetAsync(arg.OperationId) is not null) return;

        var wallet = await _uow.Wallets.GetAsync(arg.userId);

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        var balance = wallet.GetBalance(arg.currencyId);

        _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            balance.Decrease(arg.amount);

            await _uow.Transactions.AddAsync(new()
            {
                Id = arg.OperationId,
                Amount = arg.amount,
                TimeStamp = DateTime.UtcNow,
                Type = TransactionType.Withdrawal,
                UserId = arg.userId
            });

            await _uow.SaveChangesAsync();

            _uow.Commit();
        }
        catch
        {
            _uow.Rollback();
        }
    }

    public async Task CancelDeductAsync(Guid operationId)
    {
        var txn = await _uow.Transactions.GetAsync(operationId);

        if (txn is null) return;

        var wallet = await _uow.Wallets.GetAsync(txn.UserId);

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        var balance = wallet.GetBalance(txn.CurrencyId);

        _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            balance.Increase(txn.Amount);

            txn.TimeStamp = DateTime.UtcNow;
            txn.Type = TransactionType.CancelWithdrawal;

            await _uow.SaveChangesAsync();

            _uow.Commit();
        }
        catch
        {
            _uow.Rollback();
        }
    }

    public async Task UnHoldBalanceAsync(Guid userId, int currencyId)
    {
        var wallet = await _uow.Wallets.GetAsync(userId);

        if (wallet is null)
            throw new WindettaException(Errors.Wallet.NotFound);

        var balance = wallet.GetBalance(currencyId);

        balance.UnHold();

        await _uow.SaveChangesAsync();
    }
}
