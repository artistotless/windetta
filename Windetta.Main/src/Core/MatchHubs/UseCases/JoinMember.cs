﻿using Windetta.Main.Core.Exceptions;
using Windetta.Main.MatchHubs;
using Windetta.Main.MatchHubs.Filters;
using Windetta.Main.Rooms;
using Windetta.Main.Services;

namespace Windetta.Main.Core.MatchHubs.UseCases;

public class JoinMember : IJoinMemberMatchHubUseCase
{
    private readonly IMatchHubs _hubs;
    private readonly IWalletService _walletService;

    public JoinMember(IMatchHubs hubs, IWalletService walletService)
    {
        _hubs = hubs;
        _walletService = walletService;
    }

    public async Task ExecuteAsync(Guid userId, Guid hubId, Guid roomId)
    {
        var hub = await _hubs.GetAsync(hubId);

        if (hub is null)
            throw MatchHubException.NotFound;

        if (!await _walletService.IsEqualOrGreater(
            userId, hub.Bet.CurrencyId, hub.Bet.Amount))
        {
            throw WalletException.FundsNotEnough;
        }

        var joinFilters = hub.GetJoinFilters();

        if (joinFilters is not null)
            await joinFilters.ExecuteFiltersAsync(userId);

        var member = new RoomMember(userId);

        hub.Add(member, roomId);
    }
}