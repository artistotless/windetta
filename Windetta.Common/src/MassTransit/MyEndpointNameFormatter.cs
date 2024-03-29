﻿using MassTransit;
using Windetta.Contracts.Base;

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

    public string Consumer(Type consumerType)
    {
        var messageType = consumerType.GetInterfaces()[0].GenericTypeArguments[0];
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
            consumerName = $"event.consumer.{consumerType.Name.Replace("Consumer", string.Empty)}";
        }

        return $"{_defaultNamespace.ToLower()}.{consumerName.ToLower()}-{genericArgumentType.Name}";
    }

    public override string Consumer<T>()
        => Consumer(typeof(T));

    public static Uri CommandUri<T>(string serviceName)
    {
        var queueName = $"{serviceName.ToLower()}.command.consumer-{typeof(T).Name}";

        return new Uri($"queue:{queueName}");
    }
}
