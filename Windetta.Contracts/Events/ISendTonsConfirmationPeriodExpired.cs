using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface ISendTonsConfirmationPeriodExpired : CorrelatedBy<Guid>, IEvent
{
}