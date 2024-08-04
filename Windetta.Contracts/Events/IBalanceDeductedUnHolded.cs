using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

/// <summary>
/// A message informing you that the funds that were on hold have been burned up
/// </summary>
public interface IBalanceDeductedUnHeld: CorrelatedBy<Guid>, IEvent
{

}
