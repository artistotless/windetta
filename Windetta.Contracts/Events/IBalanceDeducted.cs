using MassTransit;

namespace Windetta.Contracts.Events;

public interface IBalanceDeducted : CorrelatedBy<Guid>
{

}
