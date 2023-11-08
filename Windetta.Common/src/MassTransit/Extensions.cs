using MassTransit;
using MassTransit.Metadata;
using MassTransit.SagaStateMachine;
using MassTransit.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.Messages;

namespace Windetta.Common.MassTransit;

public static class Extensions
{
    public static void AddReadyMassTransit(this IServiceCollection services,
        Assembly assembly,
        string serviceName,
        Action<IBusRegistrationConfigurator>? cfg = null)
    {
        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();

        var rmqQtions = configuration.GetOptions<RabbitMqOptions>("RabbitMq");

        services.AddMassTransit(x =>
        {
            cfg?.Invoke(x);
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
                    var formatter = Activator
                        .CreateInstance(typeof(FormatterWrapper<>)
                        .MakeGenericType(item.Item1), serviceName) as IFormatterWrapper;

                    cfg.ReceiveEndpoint(formatter.Consume(), e =>
                    {
                        e.ConfigureConsumer(context, item.Item1);

                        if (item.Item2 == MessageType.Command)
                            e.ConfigureConsumeTopology = false;
                    });
                }

                cfg.ConfigureEndpoints(context);
            });
        });
    }

    private interface IFormatterWrapper
    {
        public string Consume();
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

    private class FormatterWrapper<T> : IFormatterWrapper where T : class, IConsumer
    {
        private IEndpointNameFormatter _formatter;

        public FormatterWrapper(string serviceName)
        {
            _formatter = new MyEndpointNameFormatter(serviceName);
        }

        public string Consume()
          => _formatter.Consumer<T>();
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
