using Microsoft.EntityFrameworkCore;
using Windetta.TonTxns.Domain;

namespace Windetta.TonTxns.Data;

public sealed class TonDbContext : DbContext
{
    public TonDbContext(DbContextOptions options) : base(options)
    {
    }

    internal DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Transaction>()
            .Property(x => x.Id)
            .HasColumnType("CHAR(36)");

        modelBuilder.Entity<Transaction>()
          .Property(x => x.Status)
          .HasDefaultValue(TransactionStatus.Pending)
          .HasColumnType("TINYINT");

        modelBuilder.Entity<Transaction>()
          .Property(x => x.TransfersCount)
          .HasColumnType("SMALLINT");

        modelBuilder.Entity<Transaction>()
          .Property(x => x.TotalAmount)
          .HasColumnType("BIGINT");
    }
}