using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface INotifyProccessingWinningsFailed : CorrelatedBy<Guid>, ICommand
{
    public string FaultMessage { get; set; }
}
