using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface ITonWithdrawalStatusRequested : CorrelatedBy<Guid>, IEvent
{

}

public record TonWithdrawalStatus(ulong nanotons, int state, TonAddress destination, string? failReason = null);
public record TonWithdrawalNotfound();
