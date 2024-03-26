using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface INotifyReadyToConnect : CorrelatedBy<Guid>, ICommand
{
    public IEnumerable<Guid> Players { get; set; }
}