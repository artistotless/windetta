﻿namespace Windetta.Main.Core.Lobbies;

public struct Bet
{
    public int CurrencyId { get; init; }
    public ulong Amount { get; init; }

    public Bet(int currencyId, ulong bet)
    {
        CurrencyId = currencyId;
        Amount = bet;
    }
}