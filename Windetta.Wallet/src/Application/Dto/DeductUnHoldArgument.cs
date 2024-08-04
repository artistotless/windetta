using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record DeductUnHoldArgument(IEnumerable<BalanceOperationData> Data, NegativeBalanceOperationType Type);