namespace Windetta.Contracts;

public class FundsInfo
{
    public int CurrencyId { get; init; }
    public ulong Amount { get; init; }

    public FundsInfo(int currencyId, ulong amount)
    {
        CurrencyId = currencyId;
        Amount = amount;
    }
}

public enum PositiveBalanceOperationType
{
    TopUp,
    Winnings,
}

public enum NegativeBalanceOperationType
{
    Withdrawal,
    Loss
}