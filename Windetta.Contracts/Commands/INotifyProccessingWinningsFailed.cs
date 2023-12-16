using MassTransit;
using Windetta.Common.Messages;
using Windetta.Contracts.Events;

namespace Windetta.Contracts.Commands;

public interface INotifyProccessingWinningsFailed : CorrelatedBy<Guid>, ICommand
{
    public Fault<IMatchCompleted> FaultMessage { get; set; }
}
