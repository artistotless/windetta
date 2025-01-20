using Microsoft.EntityFrameworkCore;
using Windetta.Wallet.Application.DAL;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Infrastructure.Data.Repositories;

public sealed class WalletsRepository : IWallets
{
    private readonly WalletDbContext _dbContext;

    public WalletsRepository(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(UserWallet wallet)
    {
        _dbContext.Wallets.Add(wallet);
    }

    public async Task<IEnumerable<UserWallet>> GetAllAsync(IEnumerable<Guid> userIds)
    {
        var query = _dbContext.Wallets
            .Include(w => w.Balances)
            .Where(w => userIds.Contains(w.UserId));

        return await query.ToListAsync();
    }

    public async Task<UserWallet?> GetAsync(Guid userId)
    {
        var query = _dbContext.Wallets
            .Include(w => w.Balances)
            .Where(w => w.UserId == userId);

        return await query.FirstOrDefaultAsync();
    }
}
