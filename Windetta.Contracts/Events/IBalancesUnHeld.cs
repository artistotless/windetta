using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IBalancesUnHeld : CorrelatedBy<Guid>, IEvent
{

}