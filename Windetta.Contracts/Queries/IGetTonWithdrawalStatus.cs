using MassTransit;
using Windetta.Common.Types;

namespace Windetta.Contracts.Queries;

public interface IGetTonWithdrawalStatus : CorrelatedBy<Guid>
{

}

public record TonWithdrawalStatus(long nanotons, int state, TonAddress destination, string? failReason = null);