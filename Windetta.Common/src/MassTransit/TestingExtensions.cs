using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Windetta.Common.MassTransit;

public static class TestingExtensions
{
    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, string serviceName, Action<IBusRegistrationConfigurator>? configure = null)
    {
        services.AddQuartz(x =>
        {
            x.UseMicrosoftDependencyInjectionJobFactory();
        })
            .AddMassTransitTestHarness(x =>
            {
                x.SetEndpointNameFormatter(new MyEndpointNameFormatter(serviceName));

                x.AddQuartzConsumers();

                x.AddPublishMessageScheduler();

                configure?.Invoke(x);

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UsePublishMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            });

        return services;
    }
}
