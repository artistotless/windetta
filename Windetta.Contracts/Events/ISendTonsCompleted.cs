using MassTransit;

namespace Windetta.Contracts.Events;

public interface ISendTonsCompleted:CorrelatedBy<Guid>
{

}
