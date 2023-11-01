using MassTransit;

namespace Windetta.Contracts.Events;

public interface ITransferTonAddedToPool:CorrelatedBy<Guid>
{

}
