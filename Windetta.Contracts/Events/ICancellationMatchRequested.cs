using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface ICancellationMatchRequested : CorrelatedBy<Guid>, IEvent
{
    public string Reason { get; set; }
}
