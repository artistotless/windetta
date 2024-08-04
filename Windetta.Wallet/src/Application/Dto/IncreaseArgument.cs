using Windetta.Contracts;

namespace Windetta.Wallet.Application.Dto;

public record IncreaseArgument(IEnumerable<BalanceOperationData> Data, PositiveBalanceOperationType Type);