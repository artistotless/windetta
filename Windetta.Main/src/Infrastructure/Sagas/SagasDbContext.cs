using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Windetta.Main.Infrastructure.Sagas;
public class MatchesMap : SagaClassMap<MatchFlow>
{
    protected override void Configure(EntityTypeBuilder<MatchFlow> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState)
            .HasColumnType("TINYINT");
        entity.HasIndex(x => x.CorrelationId);
        entity.Property(x => x.CanceledReason)
            .HasColumnType("VARCHAR(32)");
        entity.Property(x => x.Endpoint)
            .UseCollation("latin1_general_ci")
            .HasColumnType("VARCHAR(42)");
        entity.Property(x => x.Players)
            .HasConversion<PlayersDbModelConverter>();
        entity.Property(x => x.Properties)
            .HasConversion<PropertiesDbModelConverter>();
        entity.Property(x => x.Tickets)
            .UseCollation("latin1_general_ci")
            .HasConversion<TicketsDbModelConverter>();
    }
}

public class SagasDbContext : SagaDbContext
{
    public SagasDbContext(DbContextOptions options) : base(options) { }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new MatchesMap();
        }
    }
}
