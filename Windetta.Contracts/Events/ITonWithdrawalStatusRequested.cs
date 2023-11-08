using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Events;

public interface ITonWithdrawalStatusRequested : CorrelatedBy<Guid>, IEvent
{

}

public record TonWithdrawalStatus(long nanotons, int state, TonAddress destination, string? failReason = null);
