using MassTransit;

namespace Windetta.Common.MassTransit;

public interface IHarnessConfiguration
{
    public Action<IBusRegistrationConfigurator> ConfigureHarness();
}
