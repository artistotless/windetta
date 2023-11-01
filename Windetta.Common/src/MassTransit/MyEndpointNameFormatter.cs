using MassTransit;

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

        var fullMessageName = $"{genericArgumentType.FullName}";

        var queueName = $"{_defaultNamespace}.consumer-{fullMessageName}".ToLowerInvariant();

        return queueName;
    }

    public Uri CommandUri<T>()
    {
        var fullMessageName = $"{typeof(T).FullName}";

        var queueName = $"{_defaultNamespace}.consumer-{fullMessageName}".ToLowerInvariant();

        return new Uri($"queue:{queueName}");
    }
}
