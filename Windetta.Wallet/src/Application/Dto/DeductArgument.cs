using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record DeductArgument(Guid userId, FundsInfo funds) : IHasOperationId
{
    public NegativeBalanceOperationType Type { get; set; }
    public Guid OperationId { get; init; }
}