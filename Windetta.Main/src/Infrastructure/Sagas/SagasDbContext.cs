using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Windetta.Main.Infrastructure.Sagas;
public class MatchFlowMap : SagaClassMap<MatchFlow>
{
    protected override void Configure(EntityTypeBuilder<MatchFlow> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState)
            .HasColumnType("TINYINT");
        entity.Property(x => x.GameServerEndpoint)
            .UseCollation("latin1_general_ci")
            .HasColumnType("VARCHAR(42)");
        entity.HasIndex(x => x.CorrelationId);
        entity.Property(x => x.Players)
            .HasConversion<PlayersDbModelConverter>();
        entity.Property(x => x.Properties)
            .HasConversion<PropertiesDbModelConverter>();
    }
}

public class LobbyFlowMap : SagaClassMap<LobbyFlow>
{
    protected override void Configure(EntityTypeBuilder<LobbyFlow> entity, ModelBuilder model)
    {
        //TODO: доделать
        entity.Property(x => x.CurrentState)
            .HasColumnType("TINYINT");
        entity.HasIndex(x => x.CorrelationId);
        entity.Property(x => x.Players)
            .HasConversion<PlayersDbModelConverter>();
        entity.Property(x => x.Properties)
            .HasConversion<PropertiesDbModelConverter>();
    }
}


public class SagasDbContext : SagaDbContext
{
    public SagasDbContext(DbContextOptions options) : base(options) { }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new MatchFlowMap();
            yield return new LobbyFlowMap();
        }
    }
}
