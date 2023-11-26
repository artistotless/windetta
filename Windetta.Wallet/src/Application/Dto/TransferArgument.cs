using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record TransferArgument(Guid userId, int currencyId, ulong amount, Guid destinationUserId)
    : IHasOperationId
{
    public Guid OperationId { get; init; }
}
