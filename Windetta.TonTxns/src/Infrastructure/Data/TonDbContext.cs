using Microsoft.EntityFrameworkCore;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Infrastructure.Data;

public sealed class TonDbContext : DbContext
{
    public TonDbContext(DbContextOptions options) : base(options)
    {
    }

    internal DbSet<Withdrawal> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Withdrawal>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Withdrawal>()
            .Property(x => x.Id)
            .HasColumnType("CHAR(36)");

        modelBuilder.Entity<Withdrawal>()
          .Property(x => x.Status)
          .HasDefaultValue(WithdrawalStatus.Pending)
          .HasColumnType("TINYINT");

        modelBuilder.Entity<Withdrawal>()
          .Property(x => x.TransfersCount)
          .HasColumnType("SMALLINT");

        modelBuilder.Entity<Withdrawal>()
          .Property(x => x.TotalAmount)
          .HasColumnType("BIGINT");
    }
}