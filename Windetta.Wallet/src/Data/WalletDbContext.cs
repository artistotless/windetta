using Microsoft.EntityFrameworkCore;
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

        modelBuilder.Entity<UserWallet>()
          .Property(x => x.UserId)
          .HasColumnType("CHAR(36)");

        modelBuilder.Entity<UserWallet>()
          .Property(x => x.HeldBalance)
          .HasColumnType("BIGINT");

        modelBuilder.Entity<UserWallet>()
          .Property(x => x.Balance)
          .HasColumnType("BIGINT");

        modelBuilder.Entity<Transaction>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Transaction>()
            .Property(x => x.Id)
            .HasColumnType("VARCHAR(40)");

        modelBuilder.Entity<Transaction>()
          .Property(x => x.TimeStamp)
          .HasColumnType("DATETIME(6)");

        modelBuilder.Entity<Transaction>()
          .Property(x => x.Amount)
          .HasColumnType("BIGINT");

        modelBuilder.Entity<Transaction>()
         .Property(x => x.Type)
         .HasColumnType("TINYINT");
    }
}