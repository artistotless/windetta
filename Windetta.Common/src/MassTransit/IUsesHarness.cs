using MassTransit;

namespace Windetta.Common.MassTransit;

public interface IUsesHarness
{
    public Action<IBusRegistrationConfigurator> ConfigureHarness();
}
