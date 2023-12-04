using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.MatchHubs;

namespace Windetta.Main.Infrastructure.SignalR;

[Authorize(Policy = "NeedRealtimeScope")]
internal class MainHub : Hub
{
    private readonly MatchHubsInteractor _interactor;
    private readonly MatchHubsDispatcher _dispatcher;

    public MainHub(MatchHubsInteractor interactor, MatchHubsDispatcher dispatcher)
    {
        _interactor = interactor;
        _dispatcher = dispatcher;
    }

    public async Task CreateMatchHub(Guid gameId, Bet bet, bool privateHub = false)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);

        var createRequest = new CreateMatchHubRequest()
        {
            Bet = bet,
            InitiatorId = userId,
            GameId = gameId,
            Private = privateHub,
        };

        var hub = await _interactor.CreateAsync(createRequest);

        _dispatcher.AddToTracking(hub);

        await Groups.AddToGroupAsync(Context.ConnectionId, hub.Id.ToString());

        await Clients.All.SendAsync("onAddedMatchHub", new MatchHubDto(hub));
    }

    public async Task JoinHub(Guid hubId, Guid roomId)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);

        await _interactor.JoinMember(userId, hubId, roomId);

        await Groups.AddToGroupAsync(Context.ConnectionId, hubId.ToString());
    }

    public async Task LeaveHub(Guid hubId)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);

        await _interactor.LeaveMember(userId, hubId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubId.ToString());
    }

    public async Task GetMatchHubs()
    {
        var hubs = await _interactor.GetAllAsync();

        await Clients.Caller.SendAsync("onReceivedMatchHubs", hubs.ToArray());
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var hubId = await _interactor.GetHubIdByUserId(userId);

        if (hubId.HasValue == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, hubId.Value.ToString());
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Guid.Parse(Context.UserIdentifier!);
        var hubId = await _interactor.GetHubIdByUserId(userId);

        if (hubId.HasValue == true)
            await _interactor.LeaveMember(userId, hubId.Value);
    }
}
