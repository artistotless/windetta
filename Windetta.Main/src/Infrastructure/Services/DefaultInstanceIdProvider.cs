using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.Services;

[ExcludeFromAutoInject]
public class DefaultInstanceIdProvider : IInstanceIdProvider
{
    private readonly string? _instanceIdValue;

    public DefaultInstanceIdProvider(string? instanceId = null)
    {
        _instanceIdValue = instanceId;
    }

    public string GetId()
    {
        var envValue = Environment.GetEnvironmentVariable("InstanceId");
        var template = (Guid id) => $"Windetta.{nameof(Svc.Main)}.{id}";

        // Get value from Environment
        if (!string.IsNullOrEmpty(envValue))
            return template(new Guid(envValue));

        // Get value from configuration
        if (!string.IsNullOrEmpty(_instanceIdValue))
            return template(new Guid(_instanceIdValue));

        // Fallback - generate new id
        return template(Guid.NewGuid());
    }
}