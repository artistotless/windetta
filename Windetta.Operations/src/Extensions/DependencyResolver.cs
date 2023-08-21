using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Constants;
using Windetta.Common.Database;
using Windetta.Common.MassTransit;
using Windetta.Common.Options;
using Windetta.Common.RabbitMQ;
using Windetta.Operations.Data;

namespace Windetta.Operations.Extensions;

public static class DependencyResolver
{

    // Configure Db connection to storing users, roles, claims and so on.
    public static void AddSagasDbContext(this IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();

        var configuration = provider.GetRequiredService<IConfiguration>();

        services.Configure<MysqlSettings>(configuration.GetSection("Mysql"));

        var settings = configuration.GetOptions<MysqlSettings>("Mysql");
        var connString = settings.GetConnectionString();

        services.AddDbContext<SagasDbContext>(options => options.UseMySql(connString, new MySqlServerVersion(settings.Version),
             b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)));
    }

    public static void ConfigureMassTransit(this IServiceCollection services)
    {
        var assembly = typeof(DependencyResolver).Assembly;
        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var rmqQtions = configuration.GetOptions<RabbitMqOptions>("RabbitMq");

        services.AddMassTransit(x =>
        {
            x.SetEntityFrameworkSagaRepositoryProvider(x =>
            {
                x.ConcurrencyMode = ConcurrencyMode.Optimistic;
                x.ExistingDbContext<SagasDbContext>();
            });

            x.SetEndpointNameFormatter(
                new MyEndpointNameFormatter(Svc.Operations));

            x.AddSagaStateMachines(assembly);
            x.AddSagas(assembly);
            x.AddConsumers(assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rmqQtions.Hostnames.First() ?? "localhost", rmqQtions.VirtualHost ?? "/", h =>
                {
                    h.Username(rmqQtions.Username ?? "admin");
                    h.Password(rmqQtions.Password ?? "admin");
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
