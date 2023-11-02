using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record TransferArgument(Guid userId, long amount, Guid destinationUser) : IHasOperationId
{
    public Guid OperationId { get; init; }
}
