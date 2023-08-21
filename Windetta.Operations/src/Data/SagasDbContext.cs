using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Windetta.Operations.Sagas;

namespace Windetta.Operations.Data;

public class UserRegistrationMap : SagaClassMap<UserRegistrationState>
{
    protected override void Configure(EntityTypeBuilder<UserRegistrationState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.UserId);
        entity.Property(x => x.WalletId);
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
            yield return new UserRegistrationMap();
        }
    }
}
