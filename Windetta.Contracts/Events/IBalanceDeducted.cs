using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

/// <summary>
/// Event message to notify the user's balance has been reduced. 
/// </summary>
public interface IBalanceDeducted : CorrelatedBy<Guid>, IEvent
{

}