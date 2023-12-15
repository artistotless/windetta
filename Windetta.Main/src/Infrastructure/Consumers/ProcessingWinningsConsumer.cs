using MassTransit;
using Windetta.Contracts.Commands;

namespace Windetta.Main.Infrastructure.Consumers;

public class ProcessingWinningsConsumer : IConsumer<IProcessWinnings>
{
    public Task Consume(ConsumeContext<IProcessWinnings> context)
    {
        throw new NotImplementedException();
    }
}
