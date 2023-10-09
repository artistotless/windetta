using Microsoft.EntityFrameworkCore;
using Windetta.Common.Helpers;
using Windetta.Common.Types;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Data;

public sealed class WalletDbContext : DbContext
{
    private readonly AesEncryptor _encryptor;

    public WalletDbContext(DbContextOptions options, AesEncryptor aes) : base(options)
    {
        _encryptor = aes;
    }

    internal DbSet<UserWallet> Wallets { get; set; }
    internal DbSet<WalletKeysSet> Credentials { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WalletKeysSet>()
         .Property(x => x.PrivateKey)
         .HasConversion(x => _encryptor.Encrypt(x), x => _encryptor.Decrypt(x));

        modelBuilder.Entity<WalletKeysSet>()
            .HasKey(x => x.UserId);

        modelBuilder.Entity<UserWallet>()
            .HasKey(x => x.UserId);

        modelBuilder.Entity<UserWallet>()
            .Property(x => x.Address)
            .HasMaxLength(48)
            .HasColumnType("CHAR")
            .HasConversion(x => x.Value, x => new TonAddress(x));

        modelBuilder.Entity<UserWallet>()
            .HasOne(x => x.WalletKeys)
            .WithOne(k => k.Wallet)
            .HasForeignKey<WalletKeysSet>(k => k.UserId);
    }
}