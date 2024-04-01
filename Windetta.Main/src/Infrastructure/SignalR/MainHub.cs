using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Infrastructure.SignalR;

[Authorize(Policy = "NeedRealtimeScope")]
public class MainHub : Hub
{
    private readonly LobbiesInteractor _lobbiesInteractor;
    private readonly FromHeaderUserIdProvider _userIdProvider;

    public MainHub(FromHeaderUserIdProvider userIdProvider, LobbiesInteractor lobbiesInteractor)
    {
        _userIdProvider = userIdProvider;
        _lobbiesInteractor = lobbiesInteractor;
    }

    public override Task OnConnectedAsync()
        => SubscribeOnCurrentLobbyEvents();

    public override Task OnDisconnectedAsync(Exception? exception)
        => UnSubscribeFromCurrentLobbyEvents();

    private async Task SubscribeOnCurrentLobbyEvents()
    {
        var userId = GetUserId();

        await Groups.AddToGroupAsync(Context.ConnectionId, userId.ToString());

        var lobbyId = await _lobbiesInteractor.GetLobbyIdByUserIdAsync(userId);

        Log.ForContext<MainHub>().Information("User connected: {0}", userId);

        if (!lobbyId.HasValue)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.Value.ToString());
    }

    private async Task UnSubscribeFromCurrentLobbyEvents()
    {
        var userId = GetUserId();

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId.ToString());

        var lobbyId = await _lobbiesInteractor.GetLobbyIdByUserIdAsync(userId);

        Log.ForContext<MainHub>().Information("User disconnected: {0}", userId);

        if (!lobbyId.HasValue)
            return;

        await _lobbiesInteractor.LeaveMemberAsync(userId, lobbyId.Value);

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
