using MassTransit;

namespace Windetta.Common.MassTransit;

public class MyEntityNameFormatter : IEntityNameFormatter
{
    private string _serviceName;

    public MyEntityNameFormatter(string serviceName)
    {
        _serviceName = serviceName;
    }

    public string FormatEntityName<T>()
    {
        var res = $"{_serviceName}.{typeof(T).Name}";

        return res;
    }
}
