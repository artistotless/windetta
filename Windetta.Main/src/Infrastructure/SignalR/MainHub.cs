using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Services;

namespace Windetta.Main.Infrastructure.SignalR;

[Authorize(Policy = "NeedRealtimeScope")]
public class MainHub : Hub
{
    private readonly ILobbyUsersAssociations _lobbiesUsersSets;
    private readonly IUserIdService _userIdProvider;

    public MainHub(
        ILobbyUsersAssociations lobbiesUsersSets,
        IUserIdService userIdProvider)
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

    private ValueTask SubscribeOnCurrentLobbyEvents()
    {
        var lobbyId = _lobbiesUsersSets.GetLobbyId(_userIdProvider.UserId);
        if (!lobbyId.HasValue)
            return ValueTask.CompletedTask;

        return new ValueTask(Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.Value.ToString()));
    }

    private ValueTask UnSubscribeOnCurrentLobbyEvents()
    {
        var lobbyId = _lobbiesUsersSets.GetLobbyId(_userIdProvider.UserId);
        if (!lobbyId.HasValue)
            return ValueTask.CompletedTask;

        return new ValueTask(Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.Value.ToString()));
    }
}
