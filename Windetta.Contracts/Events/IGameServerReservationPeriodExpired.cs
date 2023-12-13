using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IGameServerReservationPeriodExpired : CorrelatedBy<Guid>, IEvent
{
}
