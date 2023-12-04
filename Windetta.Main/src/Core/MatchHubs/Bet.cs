namespace Windetta.Main.MatchHubs;

public struct Bet
{
    public int CurrencyId { get; init; }
    public ulong Amount { get; init; }

    public Bet(int currencyId, ulong bet)
    {
        this.CurrencyId = currencyId;
        this.Amount = bet;
    }
}