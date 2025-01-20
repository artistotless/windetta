using Windetta.Contracts;
using Windetta.Wallet.Domain;

namespace Windetta.Wallet.Application;

public static class Extensions
{
    public static TransactionType ToTransactionType(this PositiveBalanceOperationType type)
    {
        return type switch
        {
            PositiveBalanceOperationType.Winnings => TransactionType.Winnings,
            PositiveBalanceOperationType.TopUp => TransactionType.TopUp,
            _ => throw new NotImplementedException()
        };
    }

    public static TransactionType ToTransactionType(this NegativeBalanceOperationType type)
    {
        return type switch
        {
            NegativeBalanceOperationType.Loss => TransactionType.Loss,
            NegativeBalanceOperationType.Withdrawal => TransactionType.Withdrawal,
            _ => throw new NotImplementedException()
        };
    }
}
