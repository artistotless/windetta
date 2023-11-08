using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface ITransferTonConfirmationPeriodExpired : CorrelatedBy<Guid>, IEvent
{
}
