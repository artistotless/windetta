using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface ILobbyReady : CorrelatedBy<Guid>, IEvent
{
    public DateTimeOffset TimeStamp { get; set; }
}