﻿using Windetta.Main.Services;
using Windetta.MainTests.Shared;
using Windetta.Wallet.Domain;

namespace Windetta.MainTests.Mocks;
public class InMemoryWalletService : IWalletService
{
    private readonly List<UserWallet> _wallets;

    public InMemoryWalletService()
    {
        var balance = new Wallet.Domain.UserBalance()
        {
            CurrencyId = 1,
            WalletId = IdExamples.UserId
        };

        balance.Increase(1000);

        var wallet = new UserWallet()
        {
            UserId = IdExamples.UserId,
            Balances = new() { balance },
        };

        _wallets = new()
        {
           wallet,
        };
    }

    public Task<Main.Services.UserBalance> GetBalance(Guid userId, int currencyId)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == currencyId);

        return Task.FromResult(new Main.Services.UserBalance() { Amount = balance.Amount, HeldAmount = balance.HeldAmount });
    }

    public Task HoldBalance(Guid userId, int currencyId, ulong amount)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == currencyId);

        balance.Hold(amount);

        return Task.CompletedTask;
    }

    public Task<bool> IsEqualOrGreater(Guid userId, int currencyId, ulong amount)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == currencyId);

        return Task.FromResult((balance.Amount - balance.HeldAmount) >= amount);
    }

    public Task UnHoldBalance(Guid userId, int currencyId)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == currencyId);

        balance.UnHold();

        return Task.CompletedTask;
    }
}