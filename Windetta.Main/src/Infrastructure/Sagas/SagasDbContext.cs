using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Windetta.Operations.Sagas;

namespace Windetta.Main.Infrastructure.Sagas;

public class MatchesMap : SagaClassMap<MatchFlow>
{
    protected override void Configure(EntityTypeBuilder<MatchFlow> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.HasIndex(x => x.CorrelationId);
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
