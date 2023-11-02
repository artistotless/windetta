using Microsoft.EntityFrameworkCore;

namespace Windetta.TonTxns.Data;

public sealed class TonDbContext : DbContext
{
    public TonDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}