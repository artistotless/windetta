using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface ICancellationMatchRequested : CorrelatedBy<Guid>, IEvent
{
    public string Reason { get; set; }
}
