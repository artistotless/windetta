using MassTransit;
using MassTransit.Metadata;
using MassTransit.SagaStateMachine;
using MassTransit.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Contracts.Base;

namespace Windetta.Common.MassTransit;
public static class Extensions
{
    public static void AddReadyMassTransit(this IServiceCollection services,
        Assembly assembly,
        string serviceName,
        Action<IBusRegistrationConfigurator>? regCfg = null,
        Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>? busCfg = null)
    {
        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();

        var rmqQtions = configuration.GetOptions<RabbitMqOptions>("RabbitMq");

        services.AddMassTransit(x =>
        {
            regCfg?.Invoke(x);
            x.SetEndpointNameFormatter(new MyEndpointNameFormatter(serviceName));
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

                var data = GetConsumersWithMessageTypes(assembly);

                foreach (var item in data)
                {
                    if (HasExcludeRegisterAttribute(item.Item1))
                        continue;

                    var formatter = new MyEndpointNameFormatter(serviceName);

                    cfg.ReceiveEndpoint(formatter.Consumer(item.Item1), e =>
                    {
                        e.ConfigureConsumer(context, item.Item1);

                        if (item.Item2 == MessageType.Command)
                            e.ConfigureConsumeTopology = false;
                    });
                }
                busCfg?.Invoke(cfg, context);
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    private static bool HasExcludeRegisterAttribute(Type consumerType)
    {
        return consumerType.GetCustomAttribute
            <ExcludeFromAutoRegisterConsumerAttribute>() is not null;
    }

    public static EventActivityBinder<TSaga, TData> SendCommandAsync<TSaga, TData, TMessage>(this EventActivityBinder<TSaga, TData> source,
            string serviceName, Func<BehaviorContext<TSaga, TData>, Task<SendTuple<TMessage>>> messageFactory,
            Action<SendContext<TMessage>> callback = null)
            where TSaga : class, SagaStateMachineInstance
            where TData : class
            where TMessage : class
    {
        var endpoint = MyEndpointNameFormatter.CommandUri<TMessage>(serviceName);

        return source.Add(new SendActivity<TSaga, TData, TMessage>(_ => endpoint, MessageFactory<TMessage>.Create(messageFactory, callback)));
    }

    public static async Task SendCommandAsync<TMessage>(this IBus bus,
         string serviceName, object values)
         where TMessage : class
    {
        var uri = MyEndpointNameFormatter.CommandUri<TMessage>(serviceName);
        var endpoint = await bus.GetSendEndpoint(uri);

        await endpoint.Send<TMessage>(values);
    }

    public static async Task SendCommandAsync<TMessage>(this ConsumeContext ctx,
      string serviceName, object values)
      where TMessage : class
    {
        var uri = MyEndpointNameFormatter.CommandUri<TMessage>(serviceName);
        var endpoint = await ctx.GetSendEndpoint(uri);

        await endpoint.Send<TMessage>(values);
    }

    private enum MessageType { Command, Event }
    private static IEnumerable<(Type, MessageType)> GetConsumersWithMessageTypes(Assembly from)
    {
        var typeSet = AssemblyTypeCache.FindTypes(new[] { from }, RegistrationMetadata.IsConsumerOrDefinition).GetAwaiter().GetResult();
        var types = typeSet.FindTypes(TypeClassification.Concrete | TypeClassification.Closed).ToArray();
        var consumerTypes = types.Where(MessageTypeCache.HasConsumerInterfaces);

        foreach (var item in consumerTypes)
        {
            var messageType = item.GetInterfaces()[0].GenericTypeArguments[0];
            var genericArgumentType = messageType;

            if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == typeof(Batch<>))
                genericArgumentType = messageType.GenericTypeArguments[0];

            if (!genericArgumentType.IsAssignableTo(typeof(IMessage)))
                throw new InvalidCastException("Specify the message as 'IEvent' or 'ICommand'");

            var type = genericArgumentType
                .IsAssignableTo(typeof(ICommand)) ? MessageType.Command : MessageType.Event;

            yield return (item, type);
        }
    }
}
