namespace Windetta.Main.Core.Games;

/// <summary>
/// Structure of the supported game currency
/// </summary>
public struct SupportedCurrency
{
    /// <summary>
    /// Currency ID
    /// </summary>
    public int CurrencyId { get; set; }

    /// <summary>
    /// Minimum bet to successfully create or join a lobby
    /// </summary>
    public ulong MinBet { get; set; }

    /// <summary>
    /// Maximum bet that can be set when creating a lobby
    /// </summary>
    public ulong MaxBet { get; set; }

    public SupportedCurrency(int currencyId, ulong minBet, ulong maxBet)
    {
        CurrencyId = currencyId;
        MinBet = minBet;
        MaxBet = maxBet;
    }
}