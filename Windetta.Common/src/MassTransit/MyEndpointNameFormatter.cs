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
        var queueName = $"{_defaultNamespace}.{KebabCaseEndpointNameFormatter.Instance.Message<T>()}".ToLowerInvariant();

        return queueName;
    }
}
