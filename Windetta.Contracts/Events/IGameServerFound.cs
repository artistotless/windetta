using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IGameServerFound : CorrelatedBy<Guid>, IEvent
{
    public Uri Endpoint { get; set; }
    public IEnumerable<Ticket> Tickets { get; set; }
}
