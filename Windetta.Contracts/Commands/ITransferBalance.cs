using MassTransit;

namespace Windetta.Contracts.Commands;

public interface ITransferBalance : CorrelatedBy<Guid>
{
    public Guid InitiatorUserId { get; set; }
    public Guid DestinationUserId { get; set; }
    public long Amount { get; set; }
}
