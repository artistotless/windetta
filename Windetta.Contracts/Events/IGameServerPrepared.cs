using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IGameServerPrepared : CorrelatedBy<Guid>, IEvent
{
    public string Endpoint { get; set; }
    public IReadOnlyDictionary<Guid, string> Tickets { get; set; }
}
