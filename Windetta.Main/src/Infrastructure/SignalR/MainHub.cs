﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Infrastructure.SignalR;

[Authorize(Policy = "NeedRealtimeScope")]
public class MainHub : Hub
{
    private readonly ILobbyUsersAssociations _lobbiesUsersSets;
    private readonly FromHeaderUserIdProvider _userIdProvider;

    public MainHub(
        ILobbyUsersAssociations lobbiesUsersSets,
        FromHeaderUserIdProvider userIdProvider)
    {
        _lobbiesUsersSets = lobbiesUsersSets;
        _userIdProvider = userIdProvider;
    }

    public async Task Subscribe()
    {
        await SubscribeOnCurrentLobbyEvents();
    }

    public override async Task OnConnectedAsync()
    {
        await SubscribeOnCurrentLobbyEvents();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await UnSubscribeOnCurrentLobbyEvents();
    }

    private async Task SubscribeOnCurrentLobbyEvents()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetUserId().ToString());

        var lobbyId = _lobbiesUsersSets.GetLobbyId(GetUserId());
        if (!lobbyId.HasValue)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.Value.ToString());
    }

    private async Task UnSubscribeOnCurrentLobbyEvents()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserId().ToString());

        var lobbyId = _lobbiesUsersSets.GetLobbyId(GetUserId());
        if (!lobbyId.HasValue)
            return;

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.Value.ToString());
    }

    private Guid GetUserId()
    {
        var feature = Context.Features
            .First(x => x.Key.Equals(typeof(IHttpContextFeature)))
            .Value as IHttpContextFeature;

        return _userIdProvider.FromContext(feature?.HttpContext);
    }
}
