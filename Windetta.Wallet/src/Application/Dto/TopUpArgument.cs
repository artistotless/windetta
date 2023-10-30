using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record TopUpArgument(Guid userId, long nanotons) : IHasOperationId
{
    public Guid OperationId { get; init; }
}
