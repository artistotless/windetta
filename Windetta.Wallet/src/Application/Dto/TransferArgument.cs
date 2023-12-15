using Windetta.Common.Types;
using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record TransferArgument(Guid userId, FundsInfo funds, Guid destinationUserId)
    : IHasOperationId
{
    public Guid OperationId { get; init; }
}
