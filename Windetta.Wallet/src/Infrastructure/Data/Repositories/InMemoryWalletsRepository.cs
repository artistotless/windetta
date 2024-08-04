using System.Collections.Concurrent;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Infrastructure.Data.Repositories;

public sealed class InMemoryWalletsRepository : IWallets
{
    private static readonly ConcurrentDictionary<Guid, UserWallet> _store;

    static InMemoryWalletsRepository()
    {
        _store = new();

        var userId1 = Guid.Parse("08dbc8b3-4170-4972-8728-c4ff931915f1");
        var userId2 = Guid.Parse("08dbc8b3-bf87-469c-8649-74c8d7b14255");

        var balance1 = new UserBalance()
        {
            WalletId = userId1,
            CurrencyId = 1,
        };

        var balance2 = new UserBalance()
        {
            WalletId = userId2,
            CurrencyId = 1,
        };

        balance1.Increase(1000000);
        balance2.Increase(1000000);

        _store.TryAdd(userId1, new UserWallet()
        {
            UserId = userId1,
            Balances = new List<UserBalance> { balance1 }
        });

        _store.TryAdd(userId2, new UserWallet()
        {
            UserId = userId2,
            Balances = new List<UserBalance> { balance2 }
        });
    }

    public void Add(UserWallet wallet)
    {
        _store.TryAdd(wallet.UserId, wallet);
    }

    public Task<IEnumerable<UserWallet>> GetAllAsync(IEnumerable<Guid> userIds)
    {
        var distincted = userIds.Distinct();
        var pairs = _store.Where(x => distincted.Contains(x.Key));

        return Task.FromResult(pairs.Select(x => x.Value));
    }

    public Task<UserWallet?> GetAsync(Guid userId)
    {
        var result = _store
            .TryGetValue(userId, out var userWallet)
            ? userWallet : null;

        return Task.FromResult(result);
    }
}
