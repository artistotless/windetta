using MassTransit;

namespace Windetta.Contracts.Events;

public interface IWithdrawPeriodExpired : CorrelatedBy<Guid>
{
}
