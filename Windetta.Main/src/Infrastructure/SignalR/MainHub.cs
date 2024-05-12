using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Windetta.Common.Types;
using Windetta.Main.Core.Lobbies;
using IUserIdProvider = Windetta.Common.Authentication.IUserIdProvider;

namespace Windetta.Main.Infrastructure.SignalR;

[Authorize(AuthenticationSchemes = nameof(RealtimeToken))]
[Authorize(Policy = "NeedRealtimeScope")]
public class MainHub : Hub
{
    private readonly LobbiesInteractor _lobbiesInteractor;
    private readonly IUserIdProvider _userIdProvider;

    private Guid UserId
    {
        get
        {
            return _userIdProvider.UserId;
        }
    }

    public MainHub(IUserIdProvider userIdProvider, LobbiesInteractor lobbiesInteractor)
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
        await Groups.AddToGroupAsync(Context.ConnectionId, UserId.ToString());

        var lobbyId = await _lobbiesInteractor.GetLobbyIdByUserIdAsync(UserId);

        Log.ForContext<MainHub>().Information("User connected: {0}", UserId);

        if (!lobbyId.HasValue)
            return;

        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.Value.ToString());
    }

    private async Task UnSubscribeFromCurrentLobbyEvents()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserId.ToString());

        var lobbyId = await _lobbiesInteractor.GetLobbyIdByUserIdAsync(UserId);

        Log.ForContext<MainHub>().Information("User disconnected: {0}", UserId);

        if (!lobbyId.HasValue)
            return;

        await _lobbiesInteractor.LeaveMemberAsync(UserId, lobbyId.Value);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.Value.ToString());
    }

    //private Guid GetUserId()
    //{
    //    var feature = Context.Features
    //        .First(x => x.Key.Equals(typeof(IHttpContextFeature)))
    //        .Value as IHttpContextFeature;

    //    return _userIdProvider.FromContext(feature?.HttpContext);
    //}
}
