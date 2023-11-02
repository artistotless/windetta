using MassTransit;

namespace Windetta.Contracts.Events;

public interface ITransferTonCompleted:CorrelatedBy<Guid>
{

}
