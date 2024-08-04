using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IBalanceIncreased : CorrelatedBy<Guid>, IEvent
{

}

