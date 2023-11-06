using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface IGetTonWithdrawalStatus : CorrelatedBy<Guid>, ICommand
{

}

public record TonWithdrawalStatus(long nanotons, int state, TonAddress destination, string? failReason = null);
