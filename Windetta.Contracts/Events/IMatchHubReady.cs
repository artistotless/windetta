using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IMatchHubReady : CorrelatedBy<Guid>, IEvent
{

}
