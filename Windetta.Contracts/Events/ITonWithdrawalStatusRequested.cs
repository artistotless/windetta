using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Events;

public interface ITonWithdrawalStatusRequested : CorrelatedBy<Guid>, IEvent
{

}

public record TonWithdrawalStatus(ulong nanotons, int state, TonAddress destination, string? failReason = null);
