using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IGameServerReservationPeriodExpired : CorrelatedBy<Guid>, IEvent
{
}
