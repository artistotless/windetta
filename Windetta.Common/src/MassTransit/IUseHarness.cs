using MassTransit;

namespace Windetta.Common.MassTransit;

public interface IUseHarness
{
    public Action<IBusRegistrationConfigurator> ConfigureHarness();
}
