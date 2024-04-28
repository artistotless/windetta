using Windetta.Common.Testing;
using Windetta.Contracts;
using Windetta.Contracts.Responses;
using Windetta.Main.Core.Services.Wallet;
using Windetta.Wallet.Domain;

namespace Windetta.MainTests.Mocks;

public class TestInMemoryWalletService : IWalletService
{
    private readonly List<UserWallet> _wallets;

    public TestInMemoryWalletService()
    {
        var balance = new UserBalance()
        {
            CurrencyId = 1,
            WalletId = ExampleGuids.UserId
        };

        balance.Increase(1000);

        var wallet = new UserWallet()
        {
            UserId = ExampleGuids.UserId,
            Balances = new() { balance },
        };

        _wallets = new()
        {
           wallet,
        };
    }

    public Task<UserBalanceResponse> GetBalance(Guid userId, int currencyId)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == currencyId);

        return Task.FromResult(new UserBalanceResponse(balance.Amount, balance.HeldAmount));
    }

    public Task<bool> IsEqualOrGreater(Guid userId, FundsInfo funds)
    {
        var wallet = _wallets.Find(w => w.UserId == userId);
        var balance = wallet.Balances.Find(b => b.CurrencyId == funds.CurrencyId);

        return Task.FromResult((balance.Amount - balance.HeldAmount) >= funds.Amount);
    }
}