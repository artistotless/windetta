using System.Data;
using Windetta.Common.Constants;
using Windetta.Contracts;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Application.Dto;
using Windetta.Wallet.Domain;
using Windetta.Wallet.Exceptions;

namespace Windetta.Wallet.Application.Services;

public class UserWalletService : IUserWalletService
{
    private readonly UnitOfWork _uow;

    public UserWalletService(UnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<UserBalance> GetBalance(Guid userId, int currencyId)
    {
        var wallet = await _uow.Wallets.Value.GetAsync(userId);

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
            {
                new()
                {
                    CurrencyId = (int)Currency.Ton,
                    WalletId = userId,
                }
            }
        };

        var foundWallet = await _uow.Wallets.Value.GetAsync(userId);

        if (foundWallet is not null)
            return;

        _uow.Wallets.Value.Add(userWallet);

        await _uow.SaveChangesAsync();
    }

    public async Task HoldBalanceAsync(Guid userId, FundsInfo funds)
    {
        var wallet = await _uow.Wallets.Value.GetAsync(userId);

        if (wallet is null)
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(funds.CurrencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

        balance.Hold(funds.Amount);

        await _uow.SaveChangesAsync();
    }

    public async Task IncreaseBalance(IncreaseArgument arg)
    {
        var wallets = await _uow.Wallets.Value.GetAllAsync(arg.Data.Select(x => x.UserId));
        var usersCount = arg.Data.Count();

        var balances = wallets.SelectMany(w => w.Balances
            .Where(b => b.CurrencyId == arg.Data.First(x => x.UserId == w.UserId).Funds.CurrencyId));

        using var transaction = _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            foreach (var operation in arg.Data)
            {
                if (await _uow.Transactions.Value.GetAsync(operation.OperationId) is not null)
                    continue;

                var wallet = wallets.FirstOrDefault(x => x.UserId == operation.UserId);

                if (wallet is null)
                    throw new WalletNotFoundException();

                var balance = wallet.GetBalance(operation.Funds.CurrencyId);

                if (balance is null)
                    throw new BalanceNotFoundException();

                balance.Increase(operation.Funds.Amount);

                _uow.Transactions.Value.Add(new()
                {
                    Id = operation.OperationId,
                    CurrencyId = operation.Funds.CurrencyId,
                    Amount = operation.Funds.Amount,
                    TimeStamp = DateTime.UtcNow,
                    Type = arg.Type.ToTransactionType(),
                    UserId = operation.UserId,
                });

                await _uow.SaveChangesAsync();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    public async Task TransferAsync(TransferArgument arg)
    {
        if ((await _uow.Transactions.Value.GetAsync(arg.OperationId)) is not null)
            return;

        var user1Wallet = await _uow.Wallets.Value.GetAsync(arg.userId);
        var user2Wallet = await _uow.Wallets.Value.GetAsync(arg.destinationUserId);

        if (user1Wallet is null || user2Wallet is null)
            throw new WalletNotFoundException();

        using var transaction = _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            user1Wallet.TransferToWallet(user2Wallet, arg.funds);

            var timestamp = DateTime.UtcNow;

            var txnOut = new Transaction()
            {
                Id = arg.OperationId,
                Amount = arg.funds.Amount,
                CurrencyId = arg.funds.CurrencyId,
                TimeStamp = timestamp,
                Type = TransactionType.TransferOut,
                UserId = arg.userId
            };

            var txnIn = new Transaction()
            {
                Id = Guid.NewGuid(),
                Amount = arg.funds.Amount,
                CurrencyId = arg.funds.CurrencyId,
                TimeStamp = timestamp,
                Type = TransactionType.TransferIn,
                UserId = arg.destinationUserId
            };

            _uow.Transactions.Value.Add(txnOut);
            _uow.Transactions.Value.Add(txnIn);

            await _uow.SaveChangesAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    public async Task DeductAsync(DeductArgument arg)
    {
        if (await _uow.Transactions.Value.GetAsync(arg.OperationId) is not null)
            return;

        var wallet = await _uow.Wallets.Value.GetAsync(arg.userId);

        if (wallet is null)
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(arg.funds.CurrencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

        using var transaction = _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            balance.Decrease(arg.funds.Amount);

            _uow.Transactions.Value.Add(new()
            {
                Id = arg.OperationId,
                Amount = arg.funds.Amount,
                CurrencyId = arg.funds.CurrencyId,
                TimeStamp = DateTime.UtcNow,
                Type = arg.Type.ToTransactionType(),
                UserId = arg.userId
            });

            await _uow.SaveChangesAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    public async Task DeductUnHoldAsync(DeductUnHoldArgument arg)
    {
        var wallets = await _uow.Wallets.Value.GetAllAsync(arg.Data.Select(x => x.UserId));
        var usersCount = arg.Data.Count();

        var balances = wallets.SelectMany(w => w.Balances
            .Where(b => b.CurrencyId == arg.Data.First(x => x.UserId == w.UserId).Funds.CurrencyId));

        using var transaction = _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            foreach (var operation in arg.Data)
            {
                if (await _uow.Transactions.Value.GetAsync(operation.OperationId) is not null)
                    continue;

                var wallet = wallets.FirstOrDefault(x => x.UserId == operation.UserId);

                if (wallet is null)
                    throw new WalletNotFoundException();

                var balance = wallet.GetBalance(operation.Funds.CurrencyId);

                if (balance is null)
                    throw new BalanceNotFoundException();

                balance.UnHold(operation.Funds.Amount);
                balance.Decrease(operation.Funds.Amount);

                await _uow.SaveChangesAsync();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    public async Task CancelDeductAsync(Guid operationId)
    {
        var txn = await _uow.Transactions.Value.GetAsync(operationId);

        if (txn is null)
            return;

        // If the transaction has already been canceled - return
        if (txn.Type == TransactionType.CancelDeduct)
            return;

        // Only the withdraw can be canceled
        if (txn.Type != TransactionType.Withdrawal)
            return;

        var wallet = await _uow.Wallets.Value.GetAsync(txn.UserId);

        if (wallet is null)
            throw new WalletNotFoundException();

        var balance = wallet.GetBalance(txn.CurrencyId);

        if (balance is null)
            throw new BalanceNotFoundException();

        using var transaction = _uow.BeginTransaction(IsolationLevel.Serializable);

        try
        {
            balance.Increase(txn.Amount);

            txn.TimeStamp = DateTime.UtcNow;
            txn.Type = TransactionType.CancelDeduct;

            await _uow.SaveChangesAsync();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
        }
    }

    public async Task UnHoldBalanceAsync(Guid userId, FundsInfo funds)
    {
        var wallet = await _uow.Wallets.Value.GetAsync(userId);

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
        var wallets = await _uow.Wallets.Value.GetAllAsync(userIds);
        var usersCount = userIds.Count();

        if (!wallets.Any() || wallets.Count() != usersCount)
            throw new WalletNotFoundException();

        var balances = wallets.SelectMany(w => w.Balances
            .Where(b => b.CurrencyId == funds.CurrencyId));

        if (balances.Count() != usersCount)
            throw new BalanceNotFoundException();

        foreach (var balance in balances)
            balance.UnHold(funds.Amount);

        await _uow.SaveChangesAsync();
    }

    public async Task HoldBalanceAsync(IEnumerable<Guid> userIds, FundsInfo funds)
    {
        var wallets = await _uow.Wallets.Value.GetAllAsync(userIds);
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
