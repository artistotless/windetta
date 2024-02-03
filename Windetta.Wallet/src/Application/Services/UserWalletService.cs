using System.Data;
using Windetta.Contracts;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Domain;
using Windetta.Wallet.Exceptions;

namespace Windetta.Wallet.Application.Services;

public class UserWalletService : IUserWalletService
{
    private readonly UnitOfWorkCommittable _uow;

    public UserWalletService(UnitOfWorkCommittable uow)
    {
        _uow = uow;
    }
    public async Task<UserBalance> GetBalance(Guid userId, int currencyId)
    {
        var wallet = await _uow.Wallets.GetAsync(userId);

        if (wallet is null)
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(currencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

        return balance;
    }

    public async Task CreateWalletAsync(Guid userId, IEnumerable<UserBalance>? initial = null)
    {
        var userWallet = new UserWallet()
        {
            UserId = userId,
            Balances = initial?.ToList() ?? new List<UserBalance>()
        };

        var foundWallet = await _uow.Wallets.GetAsync(userId);

        if (foundWallet is not null)
            return;

        await _uow.Wallets.AddAsync(userWallet);

        await _uow.SaveChangesAsync();
    }

    public async Task HoldBalanceAsync(Guid userId, FundsInfo funds)
    {
        var wallet = await _uow.Wallets.GetAsync(userId);

        if (wallet is null)
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(funds.CurrencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

        balance.Hold(funds.Amount);

        await _uow.SaveChangesAsync();
    }

    public async Task TopUpBalance(TopUpArgument arg)
    {
        if (await _uow.Transactions
            .GetAsync(arg.OperationId) is not null) return;

        var wallet = await _uow.Wallets.GetAsync(arg.userId);

        if (wallet is null)
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(arg.funds.CurrencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

        _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            balance.Increase(arg.funds.Amount);

            await _uow.Transactions.AddAsync(new()
            {
                Id = arg.OperationId,
                CurrencyId = arg.funds.CurrencyId,
                Amount = arg.funds.Amount,
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
            throw new WalletNotFoundException();

        _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            user1Wallet.TransferToWallet(user2Wallet, arg.funds);

            await _uow.Transactions.AddAsync(new()
            {
                Id = arg.OperationId,
                Amount = arg.funds.Amount,
                CurrencyId = arg.funds.CurrencyId,
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
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(arg.funds.CurrencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

        _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            balance.Decrease(arg.funds.Amount);

            await _uow.Transactions.AddAsync(new()
            {
                Id = arg.OperationId,
                Amount = arg.funds.Amount,
                CurrencyId = arg.funds.CurrencyId,
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
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(txn.CurrencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

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

    public async Task UnHoldBalanceAsync(Guid userId, FundsInfo funds)
    {
        var wallet = await _uow.Wallets.GetAsync(userId);

        if (wallet is null)
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(funds.CurrencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

        balance.UnHold(funds.Amount);

        await _uow.SaveChangesAsync();
    }

    public async Task UnHoldBalanceAsync(IEnumerable<Guid> userIds, FundsInfo funds)
    {
        var wallets = await _uow.Wallets.GetAllAsync(userIds);
        var usersCount = userIds.Count();

        if (!wallets.Any() || wallets.Count() != usersCount)
            throw new WalletNotFoundException();

        var balances = wallets.SelectMany(w => w.Balances
            .Where(b => b.CurrencyId == funds.CurrencyId));

        if (balances.Count() != usersCount)
            throw new BalanceNotFoundException();

        foreach (var item in balances)
            item.UnHold(funds.Amount);

        await _uow.SaveChangesAsync();
    }

    public async Task HoldBalanceAsync(IEnumerable<Guid> userIds, FundsInfo funds)
    {
        var wallets = await _uow.Wallets.GetAllAsync(userIds);
        var usersCount = userIds.Count();

        if (!wallets.Any() || wallets.Count() != usersCount)
            throw new WalletNotFoundException();

        var balances = wallets.SelectMany(w => w.Balances
            .Where(b => b.CurrencyId == funds.CurrencyId));

        if (balances.Count() != usersCount)
            throw new BalanceNotFoundException();

        foreach (var item in balances)
            item.Hold(funds.Amount);

        await _uow.SaveChangesAsync();
    }
}
