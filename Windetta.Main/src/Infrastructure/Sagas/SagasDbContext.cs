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
            .HasColumnType("VARCHAR(42)");
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
