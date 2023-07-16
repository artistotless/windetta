using RawRabbit.Common;
using System.Reflection;
using Windetta.Common.Helpers;

namespace Windetta.Common.RabbitMQ;

internal sealed class CustomNamingConventions : NamingConventions
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
