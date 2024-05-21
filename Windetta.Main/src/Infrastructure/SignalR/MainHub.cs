using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Windetta.Main.Core.Lobbies;
using IUserIdProvider = Windetta.Common.Authentication.IUserIdProvider;

namespace Windetta.Main.Infrastructure.SignalR;

[Authorize(AuthenticationSchemes = "FirstConnectionAuth")]
[Authorize(Policy = "RequireRealtimeScope")]
public class MainHub : Hub
{
    public static class Methods
    {
        public const string SubscribeOnLobbyFlow
            = nameof(MainHub.SubscribeOnLobbyFlow);

        public const string UnsubscribeFromLobbyFlow
            = nameof(MainHub.UnSubscribeFromLobbyFlow);
    }

    private readonly LobbiesInteractor _lobbiesInteractor;
    private readonly IUserIdProvider _userIdProvider;

    private Guid UserId => _userIdProvider.UserId;

    public MainHub(IUserIdProvider userIdProvider, LobbiesInteractor lobbiesInteractor)
    {
        _userIdProvider = userIdProvider;
        _lobbiesInteractor = lobbiesInteractor;
    }

    public override async Task OnConnectedAsync()
    {
        Log.ForContext<MainHub>().Information("User connected: {0}", UserId);

        await Groups.AddToGroupAsync(Context.ConnectionId, UserId.ToString());

        await SubscribeOnCurrentLobbyFlow();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Log.ForContext<MainHub>().Information("User disconnected: {0}", UserId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId.ToString());
    }

    public async Task SubscribeOnLobbyFlow(Guid lobbyId)
    {
        var lobbyIdResult = await _lobbiesInteractor.GetLobbyIdByUserIdAsync(UserId);

        if (!lobbyIdResult.HasValue)
            return;

        if (lobbyIdResult.Value != lobbyId)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
    }

    public Task UnSubscribeFromLobbyFlow(Guid lobbyId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString());

    private async Task SubscribeOnCurrentLobbyFlow()
    {
        var lobbyId = await _lobbiesInteractor.GetLobbyIdByUserIdAsync(UserId);

        if (!lobbyId.HasValue)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.Value.ToString());
    }
}
