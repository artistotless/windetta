namespace Windetta.Main.Games;

public struct SupportedCurrency
{
    public int CurrencyId { get; set; }
    public ulong MinBet { get; set; }
    public ulong MaxBet { get; set;}
}