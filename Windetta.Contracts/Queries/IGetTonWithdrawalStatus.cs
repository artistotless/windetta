using MassTransit;

namespace Windetta.Contracts.Queries;

public interface IGetTonWithdrawalStatus : CorrelatedBy<Guid>
{

}

public record TonWithdrawalStatus(long nanotons, int state, string? failReason = null);