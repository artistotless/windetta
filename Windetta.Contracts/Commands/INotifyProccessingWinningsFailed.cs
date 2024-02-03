using MassTransit;
using Windetta.Contracts.Base;
using Windetta.Contracts.Events;

namespace Windetta.Contracts.Commands;

public interface INotifyProccessingWinningsFailed : CorrelatedBy<Guid>, ICommand
{
    public Fault<IMatchCompleted> FaultMessage { get; set; }
}
