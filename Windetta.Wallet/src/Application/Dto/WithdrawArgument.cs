using Windetta.Common.Types;
using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record WithdrawArgument(Guid userId, long nanotons, TonAddress destinationAddress): IHasOperationId
{
    public Guid OperationId { get; init; }
}
