using RawRabbit.Common;
using System.Reflection;
using Windetta.Common.Helpers;

namespace Windetta.Common.RabbitMQ;

internal sealed class CustomNamingConventions : NamingConventions
{
    public CustomNamingConventions(string defaultNamespace)
    {
        var assemblyName = Assembly.GetEntryAssembly().GetName().Name;
        RoutingKeyConvention = type =>
        $"{GetRoutingKeyNamespace(type, defaultNamespace)}{type.Name.Underscore()}".ToLowerInvariant();
        ExchangeNamingConvention = type => GetNamespace(type, defaultNamespace).ToLowerInvariant();
        QueueNamingConvention = type => GetQueueName(assemblyName, type, defaultNamespace);
        ErrorExchangeNamingConvention = () => $"{defaultNamespace}.error";
        RetryLaterExchangeConvention = span => $"{defaultNamespace}.retry";
        RetryLaterQueueNameConvetion = (exchange, span) =>
        $"{defaultNamespace}.retry_for_{exchange.Replace(".", "_")}_in_{span.TotalMilliseconds}_ms".ToLowerInvariant();
    }

    private static string GetRoutingKeyNamespace(Type type, string defaultNamespace)
    {
        var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

        return string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";
    }

    private static string GetNamespace(Type type, string defaultNamespace)
    {
        var attr = type.GetCustomAttribute(typeof(MessageNamespaceAttribute));
        var attr2 = type.GetCustomAttribute<MessageNamespaceAttribute>();
        ;
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