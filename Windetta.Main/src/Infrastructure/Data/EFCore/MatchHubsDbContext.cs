using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Windetta.Main.Infrastructure.Data.EFCore;
using Windetta.Main.Infrastructure.Services;
using Windetta.Main.MatchHub;

namespace Windetta.Main.Infrastructure.Data.DbContexts;

public class MatchHubsDbContext : DbContext
{
    private readonly IInstanceIdProvider _idProvider;

    public DbSet<MatchHubDbModel> MatchHubs { get; set; }

    public MatchHubsDbContext(DbContextOptions options, IInstanceIdProvider idProvider) : base(options)
    {
        _idProvider = idProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchHubDbModel>()
            .HasQueryFilter(m => EF.Property<string>(m, "_instanceId") == _idProvider.GetId());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<MatchHubDbModel>().Where(e => e.State == EntityState.Added);

        foreach (var item in entries)
        {
            item.Property("_instanceId").CurrentValue = _idProvider.GetId();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
