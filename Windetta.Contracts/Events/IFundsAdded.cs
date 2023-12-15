using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Events;

public interface IFundsAdded : CorrelatedBy<Guid>, IEvent
{
    public Guid UserId { get; set; }
    public FundsInfo Funds { get; set; }
}
