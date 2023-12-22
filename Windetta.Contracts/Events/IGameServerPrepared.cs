using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IGameServerPrepared : CorrelatedBy<Guid>, IEvent
{
    public string Endpoint { get; set; }
    public IReadOnlyDictionary<Guid, string> Tickets { get; set; }
}
