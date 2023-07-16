using System.Windows.Input;
using Windetta.Common.RabbitMQ;

namespace Windetta.Common.Handlers;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command, ICorrelationContext context);
}
