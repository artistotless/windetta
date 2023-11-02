using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record DeductArgument(Guid userId, long amount): IHasOperationId
{
    public Guid OperationId { get; init; }
}
