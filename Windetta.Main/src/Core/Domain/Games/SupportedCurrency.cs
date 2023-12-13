namespace Windetta.Main.Core.Domain.Games;

public struct SupportedCurrency
{
    public int CurrencyId { get; set; }
    public ulong MinBet { get; set; }
    public ulong MaxBet { get; set; }

    public SupportedCurrency(int currencyId, ulong minBet, ulong maxBet)
    {
        CurrencyId = currencyId;
        MinBet = minBet;
        MaxBet = maxBet;
    }
}