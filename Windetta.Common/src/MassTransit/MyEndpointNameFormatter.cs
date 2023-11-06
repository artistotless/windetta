using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Common.MassTransit;

public class MyEndpointNameFormatter : DefaultEndpointNameFormatter
{
    private string _defaultNamespace;

    public MyEndpointNameFormatter(string defaultNamespace)
    {
        _defaultNamespace = defaultNamespace;
    }

    public override string Saga<T>()
    {
        var sagaName = $"{_defaultNamespace}.saga-{KebabCaseEndpointNameFormatter.Instance.Message<T>()}".ToLowerInvariant();

        return sagaName;
    }

    public override string Message<T>()
    {
        var res = base.Message<T>();

        return res;
    }

    public override string Consumer<T>()
    {
        var messageType = typeof(T).GetInterfaces()[0].GenericTypeArguments[0];
        var genericArgumentType = messageType;

        if (messageType.IsGenericType && messageType.GetGenericTypeDefinition() == typeof(Batch<>))
            genericArgumentType = messageType.GenericTypeArguments[0];

        if (!genericArgumentType.IsAssignableTo(typeof(IMessage)))
            throw new InvalidCastException("Specify the message as 'IEvent' or 'ICommand'");

        var consumerName = string.Empty;

        if (genericArgumentType.IsAssignableTo(typeof(ICommand)))
        {
            consumerName = $"command.consumer";
        }
        else if (genericArgumentType.IsAssignableTo(typeof(IEvent)))
        {
            consumerName = $"event.consumer.{typeof(T).Name.Replace("Consumer", string.Empty)}";
        }

        return $"{_defaultNamespace.ToLower()}.{consumerName.ToLower()}-{genericArgumentType.Name}";
    }

    public static Uri CommandUri<T>(string serviceName)
    {
        var queueName = $"{serviceName.ToLower()}.command.consumer-{typeof(T).Name}";

        return new Uri($"queue:{queueName}");
    }
}
