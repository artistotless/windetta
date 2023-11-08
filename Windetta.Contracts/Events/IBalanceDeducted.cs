using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IBalanceDeducted : CorrelatedBy<Guid>, IEvent
{

}
