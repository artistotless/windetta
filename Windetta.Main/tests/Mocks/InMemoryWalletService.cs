﻿using Windetta.Common.Types;
using Windetta.Main.Core.Services.Wallet;

namespace Windetta.MainTests.Mocks;
public class InMemoryWalletService : IWalletService
{
    private readonly List<Wallet.Domain.UserWallet> _wallets;

    public InMemoryWalletService()
    {
        var balance = new Wallet.Domain.UserBalance()
        {
            CurrencyId = 1,
            WalletId = IdExamples.UserId
        };

        balance.Increase(1000);

        var wallet = new Wallet.Domain.UserWallet()
        {
            UserId = IdExamples.UserId,
            Balances = new() { balance },
        };

        _wallets = new()
        {
           wallet,
        };
    }

    public Task<UserBalance> GetBalance(Guid userId, int currencyId)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == currencyId);

        return Task.FromResult(new UserBalance() { Amount = balance.Amount, HeldAmount = balance.HeldAmount });
    }

    public Task HoldBalance(Guid userId, FundsInfo funds)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == funds.CurrencyId);

        balance.Hold(funds.Amount);

        return Task.CompletedTask;
    }

    public Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == funds.CurrencyId);

        return Task.FromResult((balance.Amount - balance.HeldAmount) >= funds.Amount);
    }

    public Task UnHoldBalance(Guid userId, int currencyId)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == currencyId);

        balance.UnHold();

        return Task.CompletedTask;
    }
}