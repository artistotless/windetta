using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Windetta.Contracts.Commands;
using Windetta.Main.Infrastructure.SignalR;

namespace Windetta.Main.Infrastructure.MatchHub;

public class MatchBegunNotifier : IConsumer<INotifyMatchBegun>
{
    private readonly ILogger<MatchBegunNotifier> _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchBegunNotifier(
    ILogger<MatchBegunNotifier> logger,
    IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyMatchBegun> context)
    {
        _logger.LogDebug("Saga Event: {event}", "match_begun");

        return _context.Clients
            .Group(context.Message.CorrelationId.ToString())
            .SendAsync("onMatchBegun", context.Message);
    }
}

public class ServerFoundNotifier : IConsumer<INotifyServerFound>
{
    private readonly ILogger<ServerFoundNotifier> _logger;
    private readonly IHubContext<MainHub> _context;

    public ServerFoundNotifier(
    ILogger<ServerFoundNotifier> logger,
    IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyServerFound> context)
    {
        _logger.LogDebug("Saga Event: {event}", "server_found");

        return _context.Clients
            .Group(context.Message.CorrelationId.ToString())
            .SendAsync("onServerFound");
    }
}

public class MatchCanceledNotifier : IConsumer<INotifyMatchCanceled>
{
    private readonly ILogger<MatchCanceledNotifier> _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchCanceledNotifier(
    ILogger<MatchCanceledNotifier> logger,
    IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public Task Consume(ConsumeContext<INotifyMatchCanceled> context)
    {
        _logger.LogDebug("Saga Event: {event}", "server_found");

        return _context.Clients
            .Group(context.Message.CorrelationId.ToString())
            .SendAsync("onServerFound");
    }
}

public class MatchAwaitingExpiredNotifier : IConsumer<INotifyMatchAwaitingExpired>
{
    private readonly ILogger<MatchAwaitingExpiredNotifier> _logger;
    private readonly IHubContext<MainHub> _context;

    public MatchAwaitingExpiredNotifier(
        ILogger<MatchAwaitingExpiredNotifier> logger,
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
