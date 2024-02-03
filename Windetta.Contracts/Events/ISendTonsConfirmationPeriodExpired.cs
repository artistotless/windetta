using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface ISendTonsConfirmationPeriodExpired : CorrelatedBy<Guid>, IEvent
{
}