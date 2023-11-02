using MassTransit;

namespace Windetta.Contracts.Events;

public interface IFundsAdded : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public long Amount { get; set; }
}
