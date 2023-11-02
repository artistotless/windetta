using MassTransit;

namespace Windetta.Contracts.Events;

public interface ITransferTonConfirmationPeriodExpired : CorrelatedBy<Guid>
{
}
