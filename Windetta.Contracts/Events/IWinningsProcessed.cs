using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IWinningsProcessed : CorrelatedBy<Guid>, IEvent
{

}

