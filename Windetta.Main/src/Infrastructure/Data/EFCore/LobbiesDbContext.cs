using Microsoft.EntityFrameworkCore;
using Windetta.Main.Infrastructure.Data.EFCore;
using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Infrastructure.Data.DbContexts;

public class LobbiesDbContext : DbContext
{
    private readonly IInstanceIdProvider _idProvider;

    public DbSet<LobbyDbModel> Lobbies { get; set; }

    public LobbiesDbContext(DbContextOptions options, IInstanceIdProvider idProvider) : base(options)
    {
        _idProvider = idProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LobbyDbModel>()
            .HasQueryFilter(m => EF.Property<string>(m, "_instanceId") == _idProvider.GetId());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<LobbyDbModel>().Where(e => e.State == EntityState.Added);

        foreach (var item in entries)
        {
            item.Property("_instanceId").CurrentValue = _idProvider.GetId();
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
