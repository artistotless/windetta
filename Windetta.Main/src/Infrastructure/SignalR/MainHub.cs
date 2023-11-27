using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Games;
using Windetta.Main.MatchHub;

namespace Windetta.Main.Infrastructure.SignalR;

internal class MainHub : Hub
{
    private readonly ILogger<MainHub> _logger;
    private readonly IGames _games;
    private readonly MatchHubsInteractor _interactor;
    private readonly MatchHubsDispatcher _dispatcher;

    public MainHub(IGames games,
        MatchHubsInteractor interactor,
        MatchHubsDispatcher dispatcher,
        ILogger<MainHub> logger)
    {
        _games = games;
        _interactor = interactor;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public async Task CreateMatchHub(Guid gameId, Bet bet, bool privateHub = false)
    {
        var game = await _games.GetAsync(gameId);

        if (game is null)
        {
            await Clients.Caller.SendCoreAsync("onError", new[] { "A game with this id does not exist" });

            return;
        }

        var options = new MatchHubOptions()
        {
            Bet = bet,
            Private = privateHub,
            GameConfiguration = game.Configuration
        };

        var userId = Guid.Parse(Context.UserIdentifier ?? Guid.NewGuid().ToString());

        var hub = await _interactor.CreateAsync(options);

        _dispatcher.AddToTracking(hub);

        await _interactor.JoinMember(userId, hub);

        await Groups.AddToGroupAsync(Context.ConnectionId, hub.Id.ToString());
    }

    public async Task GetMatchHubs()
    {
        var hubs = await _interactor.GetAllAsync();

        await Clients.Caller.SendCoreAsync("onReceiveMatchHubs", hubs.ToArray());
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation($"Connected: {Context.ConnectionId} \n UserId: {Context.UserIdentifier}");

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"Disconnected: {Context.ConnectionId} \n UserId: {Context.UserIdentifier}");

        return base.OnDisconnectedAsync(exception);
    }
}
