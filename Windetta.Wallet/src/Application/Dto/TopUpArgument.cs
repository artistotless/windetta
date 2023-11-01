using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record TopUpArgument(Guid userId, long amount) : IHasOperationId
{
    public Guid OperationId { get; init; }
}
