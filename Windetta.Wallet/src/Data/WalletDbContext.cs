using Microsoft.EntityFrameworkCore;
using Windetta.Common.Helpers;
using Windetta.Common.Types;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Data;

public sealed class WalletDbContext : DbContext
{
    public WalletDbContext(DbContextOptions options) : base(options)
    {
    }

    internal DbSet<UserWallet> Wallets { get; set; }
    internal DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserWallet>()
            .HasKey(x => x.UserId);
    }
}