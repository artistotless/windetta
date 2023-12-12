using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Windetta.TonTxns.Infrastructure.Sagas;

public class TonWithdrawMap : SagaClassMap<TonWithdrawFlow>
{
    protected override void Configure(EntityTypeBuilder<TonWithdrawFlow> entity, ModelBuilder model)
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
            yield return new TonWithdrawMap();
        }
    }
}
