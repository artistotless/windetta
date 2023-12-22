using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IBalanceDeducted : CorrelatedBy<Guid>, IEvent
{

}