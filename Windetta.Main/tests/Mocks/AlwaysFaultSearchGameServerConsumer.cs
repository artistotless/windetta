using MassTransit;
using Windetta.Contracts.Commands;

namespace Windetta.MainTests.Mocks;

public class AlwaysFaultSearchGameServerConsumer : IConsumer<ISearchGameServer>
{
    public Task Consume(ConsumeContext<ISearchGameServer> context)
    {
        throw new Exception();
    }
}