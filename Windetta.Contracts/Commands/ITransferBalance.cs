using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface ITransferBalance : CorrelatedBy<Guid>, ICommand
{
    public Guid InitiatorUserId { get; set; }
    public Guid DestinationUserId { get; set; }
    public long Amount { get; set; }
}
