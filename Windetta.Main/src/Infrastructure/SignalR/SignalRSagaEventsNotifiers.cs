using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Windetta.Contracts.Commands;
using Windetta.Main.Infrastructure.SignalR;
using ILogger = Serilog.ILogger;

namespace Windetta.Main.Infrastructure.Lobby;

public class SignalRSagaEventsNotifiers { }

public class MatchBegunNotifier : IConsumer<INotifyReadyToConnect>
{
    private readonly ILogger _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchBegunNotifier(IHubContext<MainHub> context)
    {
        _logger = Log.ForContext<SignalRSagaEventsNotifiers>();
        _context = context;
    }

    public async Task Consume(ConsumeContext<INotifyReadyToConnect> context)
    {
        _logger.Debug("Saga Event: {event}", "ready_connect");

        Task GetPersonalNotifyTask(Guid userId)
        {
            return _context.Clients
             .Group(userId.ToString())
             .SendAsync("onReadyToConnect", new { context.Message.GameId });
        }

        var notifyTasks = context.Message.PlayersIds
            .Select(GetPersonalNotifyTask);

        await Task.WhenAll(notifyTasks);
    }
}

public class ServerFoundNotifier : IConsumer<INotifyServerFound>
{
    private readonly ILogger _logger;
    private readonly IHubContext<MainHub> _context;

    public ServerFoundNotifier(IHubContext<MainHub> context)
    {
        _logger = Log.ForContext<SignalRSagaEventsNotifiers>();
        _context = context;
    }

    public async Task Consume(ConsumeContext<INotifyServerFound> context)
    {
        _logger.Debug("Saga Event: {event}", "server_found");

        await _context.Clients.Group(context.Message.LobbyId.ToString())
         .SendAsync("onServerFound");
    }
}

public class MatchCanceledNotifier : IConsumer<INotifyMatchCanceled>
{
    private readonly ILogger _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchCanceledNotifier(IHubContext<MainHub> context)
    {
        _logger = Log.ForContext<SignalRSagaEventsNotifiers>();
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyMatchCanceled> context)
    {
        _logger.Debug("Saga Event: {event}", "match_canceled");

        return _context.Clients
            .Group(context.Message.CorrelationId.ToString())
            .SendAsync("onMatchCanceled");
    }
}

public class MatchAwaitingExpiredNotifier : IConsumer<INotifyMatchAwaitingExpired>
{
    private readonly ILogger _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchAwaitingExpiredNotifier(IHubContext<MainHub> context)
    {
        _logger = Log.ForContext<SignalRSagaEventsNotifiers>();
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyMatchAwaitingExpired> context)
    {
        _logger.Debug("Saga Event: {event}", "awaiting_expired");

        return _context.Clients
            .Group(context.Message.CorrelationId.ToString())
            .SendAsync("onAwaitingExpired");
    }
}
