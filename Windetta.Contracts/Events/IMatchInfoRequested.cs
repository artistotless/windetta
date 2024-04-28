using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IMatchInfoRequested : CorrelatedBy<Guid>, IEvent
{

}

