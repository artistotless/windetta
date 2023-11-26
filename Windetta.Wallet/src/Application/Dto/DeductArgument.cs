using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record DeductArgument(Guid userId, int currencyId, ulong amount) : IHasOperationId
{
    public Guid OperationId { get; init; }
}
