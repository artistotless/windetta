using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Infrastructure.SignalR;

[Authorize(Policy = "NeedRealtimeScope")]
public class MainHub : Hub
{
    private readonly LobbiesInteractor _interactor;
    private readonly LobbyObserver _dispatcher;

    public MainHub(LobbiesInteractor interactor, LobbyObserver dispatcher)
    {
        _interactor = interactor;
        _dispatcher = dispatcher;
    }

    public async Task CreateLobby(CreateLobbyRequestDto request)
    {
        var createRequest = new CreateLobbyRequest()
        {
            Bet = request.Bet,
            InitiatorId = GetUserId(),
            GameId = request.GameId,
            Private = request.Private,
            JoinFilters = request.JoinFilters,
            AutoDisposeStrategy = request.AutoDisposeStrategy,
            AutoReadyStrategy = request.AutoReadyStrategy,
        };

        var lobby = await _interactor.CreateAsync(createRequest);

        _dispatcher.AddToTracking(lobby);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobby.Id.ToString());

        await Clients.All.SendAsync("onAddedLobby", new LobbyDto(lobby));
    }

    public async Task JoinLobby(Guid lobbyId, ushort roomIndex)
    {
        await _interactor.JoinMemberAsync(GetUserId(), lobbyId, roomIndex);

        await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.ToString());
    }

    public async Task LeaveLobby(Guid lobbyId)
    {
        await _interactor.LeaveMemberAsync(GetUserId(), lobbyId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId.ToString());
    }

    public async Task GetLobbies()
    {
        var lobbies = await _interactor.GetAllAsync();

        await Clients.Caller.SendAsync("onReceivedLobbies", lobbies.ToArray());
    }

    public override async Task OnConnectedAsync()
    {
        var lobbyId = await _interactor.GetLobbyIdByUserIdAsync(GetUserId());

        if (lobbyId.HasValue == true)
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId.Value.ToString());

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
        var lobbyId = await _interactor.GetLobbyIdByUserIdAsync(userId);

        if (lobbyId.HasValue == true)
            await _interactor.LeaveMemberAsync(userId, lobbyId.Value);
    }
}