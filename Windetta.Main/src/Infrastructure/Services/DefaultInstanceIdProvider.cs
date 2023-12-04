using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.Services;

[AutoInjectExclude]
public class DefaultInstanceIdProvider : IInstanceIdProvider
{
    private string? _instanceId;

    public DefaultInstanceIdProvider(string? instanceId = null)
    {
        _instanceId = instanceId;
    }

    public string GetId()
    {
        var envValue = Environment.GetEnvironmentVariable("InstanceId");
        var template = (Guid id) => $"Windetta.{nameof(Svc.Main)}.{id}";

        // Get value from Environment
        if (!string.IsNullOrEmpty(envValue))
            return template(new Guid(envValue));

        // Get value from configuration
        if (!string.IsNullOrEmpty(_instanceId))
            return template(new Guid(_instanceId));

        // Fallback - generate new id
        var generatedId = Guid.NewGuid();
        _instanceId = generatedId.ToString();

        return template(generatedId);
    }
}