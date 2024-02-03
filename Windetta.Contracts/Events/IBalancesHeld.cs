using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IBalancesHeld : CorrelatedBy<Guid>, IEvent
{

}