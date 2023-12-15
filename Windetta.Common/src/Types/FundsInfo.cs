namespace Windetta.Common.Types;

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