namespace Windetta.Common.RabbitMQ;

using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.Autofac;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;
using RawRabbit.Pipe;
using RawRabbit.Pipe.Middleware;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windetta.Common.Configuration;
using Windetta.Common.Handlers;
using Windetta.Common.Helpers;

public static class Extensions
{
    public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
        => new BusSubscriber(app);

    public static void AddRabbitMq(this ContainerBuilder builder)
    {
        RabbitMqOptions options = null;

        builder.Register(context =>
        {
            var configuration = context.Resolve<IConfiguration>();
            options = configuration.GetOptions<RabbitMqOptions>("rabbitMq");

            return options;

        }).SingleInstance();

        builder.Register(context =>
        {
            var configuration = context.Resolve<IConfiguration>();
            var options = configuration.GetOptions<RawRabbitConfiguration>("rabbitMq");

            return options;

        }).SingleInstance();

        var assembly = Assembly.GetCallingAssembly();
        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(IEventHandler<>))
            .InstancePerDependency();
        builder.RegisterAssemblyTypes(assembly)
            .AsClosedTypesOf(typeof(ICommandHandler<>))
            .InstancePerDependency();
        builder.RegisterType<BusPublisher>().As<IBusPublisher>()
            .InstancePerDependency();

        ConfigureBus(builder, options!);
    }

    private static void ConfigureBus(ContainerBuilder builder, RabbitMqOptions options)
    {
        builder.RegisterRawRabbit(new()
        {
            ClientConfiguration = null,
            DependencyInjection = ioc =>
            {
                ioc.AddSingleton(options);
                ioc.AddSingleton(options as RawRabbitConfiguration);
                ioc.AddSingleton<INamingConventions>(new CustomNamingConventions(options.Namespace));
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
    }

    private class CustomNamingConventions : NamingConventions
    {
        public CustomNamingConventions(string defaultNamespace)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
            ExchangeNamingConvention = type => GetNamespace(type, defaultNamespace).ToLowerInvariant();
            QueueNamingConvention = type => GetQueueName(assemblyName, type, defaultNamespace);
            ErrorExchangeNamingConvention = () => $"{defaultNamespace}.error";
            RetryLaterExchangeConvention = span => $"{defaultNamespace}.retry";
        }

        private static string GetNamespace(Type type, string defaultNamespace)
        {
            var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

            return string.IsNullOrWhiteSpace(@namespace) ? type.Name.Underscore() : $"{@namespace}";
        }

        private static string GetQueueName(string assemblyName, Type type, string defaultNamespace)
        {
            var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;
            var separatedNamespace = string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";

            return $"{assemblyName}/{separatedNamespace}{type.Name.Underscore()}".ToLowerInvariant();
        }
    }

    private class RetryStagedMiddleware : StagedMiddleware
    {
        public override string StageMarker { get; } = RawRabbit.Pipe.StageMarker.MessageDeserialized;

        public override async Task InvokeAsync(IPipeContext context,
            CancellationToken token = new CancellationToken())
        {
            var retry = context.GetRetryInformation();
            if (context.GetMessageContext() is CorrelationContext message)
                message.Retries = retry.NumberOfRetries;

            await Next.InvokeAsync(context, token);
        }
    }

    private static IClientBuilder UpdateRetryInfo(this IClientBuilder clientBuilder)
    {
        clientBuilder.Register(c => c.Use<RetryStagedMiddleware>());

        return clientBuilder;
    }
}
