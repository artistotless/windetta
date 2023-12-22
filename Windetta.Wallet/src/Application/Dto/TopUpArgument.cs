using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record TopUpArgument(Guid userId, FundsInfo funds) : IHasOperationId
{
    public Guid OperationId { get; init; }
}
