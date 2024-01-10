using MassTransit;
using System.Reflection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts.Events;
using Windetta.Main.Infrastructure.Sagas;

namespace Windetta.Main.Infrastructure.MassTransit;

public static class MassTransitConfiguration
{
    public static void Configure(IServiceCollection services, Assembly assembly)
    {
        services.AddReadyMassTransit(assembly, Svc.Main, cfg =>
        {
            cfg.AddRequestClient<IGameServerRequested>();
            cfg.SetEntityFrameworkSagaRepositoryProvider(x =>
            {
                x.ConcurrencyMode = ConcurrencyMode.Optimistic;
                x.ExistingDbContext<SagasDbContext>();
            });
        }, busCfg: (cfg, context) =>
        {
            cfg.Send<IGameServerRequested>(x =>
            {
                x.UseRoutingKeyFormatter(context => context.Message.LspmIp);
            });

            cfg.Publish<IGameServerRequested>(x => x.ExchangeType = "direct");
        });
    }
}