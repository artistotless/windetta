using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IFundsAdded : CorrelatedBy<Guid>, IEvent
{
    public Guid UserId { get; set; }
    public ulong Amount { get; set; }
    public int CurrencyId { get; set; }
}
