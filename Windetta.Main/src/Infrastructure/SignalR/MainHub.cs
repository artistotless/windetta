using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Dtos;

namespace Windetta.Main.Infrastructure.SignalR;

[Authorize(Policy = "NeedRealtimeScope")]
public class MainHub : Hub
{
    private readonly MatchHubsInteractor _interactor;
    private readonly MatchHubObserver _dispatcher;

    public MainHub(MatchHubsInteractor interactor, MatchHubObserver dispatcher)
    {
        _interactor = interactor;
        _dispatcher = dispatcher;
    }

    public async Task CreateMatchHub(CreateMatchHubRequestDto request)
    {
        var createRequest = new CreateMatchHubRequest()
        {
            Bet = request.Bet,
            InitiatorId = GetUserId(),
            GameId = request.GameId,
            Private = request.Private,
            JoinFilters = request.JoinFilters,
            AutoDisposeStrategy = request.AutoDisposeStrategy,
            AutoReadyStrategy = request.AutoReadyStrategy,
        };

        var hub = await _interactor.CreateAsync(createRequest);

        _dispatcher.AddToTracking(hub);

        await Groups.AddToGroupAsync(Context.ConnectionId, hub.Id.ToString());

        await Clients.All.SendAsync("onAddedMatchHub", new MatchHubDto(hub));
    }

    public async Task JoinHub(Guid hubId, ushort roomIndex)
    {
        await _interactor.JoinMemberAsync(GetUserId(), hubId, roomIndex);

        await Groups.AddToGroupAsync(Context.ConnectionId, hubId.ToString());
    }

    public async Task LeaveHub(Guid hubId)
    {
        await _interactor.LeaveMemberAsync(GetUserId(), hubId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, hubId.ToString());
    }

    public async Task GetMatchHubs()
    {
        var hubs = await _interactor.GetAllAsync();

        await Clients.Caller.SendAsync("onReceivedMatchHubs", hubs.ToArray());
    }

    public override async Task OnConnectedAsync()
    {
        var hubId = await _interactor.GetHubIdByUserIdAsync(GetUserId());

        if (hubId.HasValue == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, hubId.Value.ToString());

        // TODO: delete. test only
        //Context.Items.Add("id", Guid.NewGuid());
    }

    private Guid GetUserId()
    {
        return Guid.Parse(Context.UserIdentifier!);

        // TODO: delete. test only
        //return Context.Items.TryGetValue("id", out var value) ? (Guid)value : Guid.Parse(Context.UserIdentifier!);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetUserId();
        var hubId = await _interactor.GetHubIdByUserIdAsync(userId);

        if (hubId.HasValue == true)
            await _interactor.LeaveMemberAsync(userId, hubId.Value);
    }
}