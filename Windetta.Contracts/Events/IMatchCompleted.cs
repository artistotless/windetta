using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IMatchCompleted : CorrelatedBy<Guid>, IEvent
{
    public IEnumerable<Guid> Winners { get; set; }
}
