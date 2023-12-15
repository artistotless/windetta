using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface ITransferBalance : CorrelatedBy<Guid>, ICommand
{
    public Guid InitiatorUserId { get; set; }
    public Guid DestinationUserId { get; set; }
    public FundsInfo Funds { get; set; }
}
