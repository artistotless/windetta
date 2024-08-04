using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IFundsAdded : CorrelatedBy<Guid>, IEvent
{
    public Guid UserId { get; set; }
    public FundsInfo Funds { get; set; }
}