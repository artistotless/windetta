using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Handlers;

namespace Windetta.Common.RabbitMQ;

public static class Extensions
{
    public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
        => new BusSubscriber(app);

    public static void AddRabbitMq(this ContainerBuilder builder)
    {
        RegisterRabbitOptions(builder);
        RegisterHandlers(builder);
        ConfigureBus(builder);
    }

    private static void RegisterRabbitOptions(ContainerBuilder builder)
    {
        builder.Register(context =>
        {
            var configuration = context.Resolve<IConfiguration>();
            var options = configuration.GetOptions<RabbitMqOptions>("RabbitMq");

            return options;

        }).SingleInstance();

        builder.Register(context =>
        {
            var configuration = context.Resolve<IConfiguration>();
            var options = configuration.GetOptions<RawRabbitConfiguration>("RabbitMq");

            return options;

        }).SingleInstance();
    }

    private static void RegisterHandlers(ContainerBuilder builder)
    {
        var assembly = Assembly.GetEntryAssembly();

        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(IEventHandler<>))
            .InstancePerDependency();

        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(ICommandHandler<>))
            .InstancePerDependency();

        builder.RegisterType<BusPublisher>().As<IBusPublisher>()
            .InstancePerDependency();
    }

    private static void ConfigureBus(ContainerBuilder builder)
    {
        builder.Register<IInstanceFactory>(context =>
        {
            var options = context.Resolve<RabbitMqOptions>();
            var cfg = options as RawRabbitConfiguration;
            var namingConventions = new CustomNamingConventions(options.Namespace);

            return RawRabbitFactory.CreateInstanceFactory(new()
            {
                ClientConfiguration = cfg,
                DependencyInjection = ioc =>
                {
                    ioc.AddSingleton(options);
                    ioc.AddSingleton(options as RawRabbitConfiguration);
                    ioc.AddSingleton<INamingConventions>(namingConventions);
                },
                Plugins = p =>
                {
                    p.UseAttributeRouting();
                    p.UseRetryLater();
                    p.UpdateRetryInfo();
                    p.UseMessageContext<CorrelationContext>();
                    p.UseContextForwarding();
                }
            });

        }).SingleInstance();

        builder.Register(context => context.Resolve<IInstanceFactory>().Create());
    }

    private static IClientBuilder UpdateRetryInfo(this IClientBuilder clientBuilder)
    {
        clientBuilder.Register(c => c.Use<RetryStagedMiddleware>());

        return clientBuilder;
    }
}
