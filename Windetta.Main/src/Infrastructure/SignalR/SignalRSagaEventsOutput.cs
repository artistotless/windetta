using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Windetta.Contracts.Commands;
using Windetta.Main.Infrastructure.SignalR;
using ILogger = Serilog.ILogger;

namespace Windetta.Main.Infrastructure.Lobby;

public class SignalRSagaEventsOutput { }

public class MatchBegunNotifier : IConsumer<INotifyReadyToConnect>
{
    private readonly ILogger _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchBegunNotifier(IHubContext<MainHub> context)
    {
        _logger = Log.ForContext<SignalRSagaEventsOutput>();
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyReadyToConnect> context)
    {
        _logger.Debug("Saga Event: {event}", "ready_connect");

        Task GetPersonalNotifyTask(Guid userId)
        {
            return _context.Clients
             .Group(userId.ToString())
             .SendAsync("onReadyToConnect", context.Message.CorrelationId);
        }

        var notifyTasks = context.Message.PlayersIds
            .Select(GetPersonalNotifyTask);

        return Task.WhenAll(notifyTasks);
    }
}

public class ServerFoundNotifier : IConsumer<INotifyServerFound>
{
    private readonly ILogger _logger;
    private readonly IHubContext<MainHub> _context;

    public ServerFoundNotifier(IHubContext<MainHub> context)
    {
        _logger = Log.ForContext<SignalRSagaEventsOutput>();
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
        _logger = Log.ForContext<SignalRSagaEventsOutput>();
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyMatchCanceled> context)
    {
        _logger.Debug("Saga Event: {event}", "match_canceled");

        Task GetPersonalNotifyTask(Guid userId)
        {
            return _context.Clients
             .Group(userId.ToString())
             .SendAsync("onMatchCanceled", context.Message.Reason);
        }

        var notifyTasks = context.Message.UsersIds
            .Select(GetPersonalNotifyTask);

        return Task.WhenAll(notifyTasks);
    }
}