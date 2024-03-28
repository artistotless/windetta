using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Windetta.Contracts.Commands;
using Windetta.Main.Infrastructure.SignalR;

namespace Windetta.Main.Infrastructure.Lobby;

public class SignalRSagaEventsNotifiers { }

public class MatchBegunNotifier : IConsumer<INotifyReadyToConnect>
{
    private readonly ILogger<SignalRSagaEventsNotifiers> _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchBegunNotifier(
    ILogger<SignalRSagaEventsNotifiers> logger,
    IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<INotifyReadyToConnect> context)
    {
        _logger.LogDebug("Saga Event: {event}", "ready_connect");

        Task GetPersonalNotifyTask(Guid userId)
        {
            return _context.Clients
             .Group(userId.ToString())
             .SendAsync("onReadyToConnect");
        }

        var notifyTasks = context.Message.Players
            .Select(GetPersonalNotifyTask);

        await Task.WhenAll(notifyTasks);
    }
}

public class ServerFoundNotifier : IConsumer<INotifyServerFound>
{
    private readonly ILogger<SignalRSagaEventsNotifiers> _logger;
    private readonly IHubContext<MainHub> _context;

    public ServerFoundNotifier(
    ILogger<SignalRSagaEventsNotifiers> logger,
    IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<INotifyServerFound> context)
    {
        _logger.LogDebug("Saga Event: {event}", "server_found");

        // TODO: implement it
    }
}

public class MatchCanceledNotifier : IConsumer<INotifyMatchCanceled>
{
    private readonly ILogger<SignalRSagaEventsNotifiers> _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchCanceledNotifier(
    ILogger<SignalRSagaEventsNotifiers> logger,
    IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyMatchCanceled> context)
    {
        _logger.LogDebug("Saga Event: {event}", "match_canceled");

        return _context.Clients
            .Group(context.Message.CorrelationId.ToString())
            .SendAsync("onMatchCanceled");
    }
}

public class MatchAwaitingExpiredNotifier : IConsumer<INotifyMatchAwaitingExpired>
{
    private readonly ILogger<SignalRSagaEventsNotifiers> _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchAwaitingExpiredNotifier(
        ILogger<SignalRSagaEventsNotifiers> logger,
        IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyMatchAwaitingExpired> context)
    {
        _logger.LogDebug("Saga Event: {event}", "awaiting_expired");

        return _context.Clients
            .Group(context.Message.CorrelationId.ToString())
            .SendAsync("onAwaitingExpired");
    }
}
