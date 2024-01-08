using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.Infrastructure.SignalR;

namespace Windetta.Main.Infrastructure.MatchHub;

public class SignalRMatchHubEventsOutput : IMatchHubObserverOutput
{
    private readonly ILogger<SignalRMatchHubEventsOutput> _logger;
    private readonly IHubContext<MainHub> _context;

    public SignalRMatchHubEventsOutput(
        ILogger<SignalRMatchHubEventsOutput> logger, 
        IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task WriteHubDeleted(IMatchHub hub)
    {
        _logger.LogDebug($"Hub deleted: {hub.Id}");

        await _context.Clients.All.SendAsync("onDeletedMatchHub", hub.Id.ToString());
    }

    public async Task WriteHubReady(IMatchHub hub)
    {
        _logger.LogDebug($"Hub ready: {hub.Id}");

        await _context.Clients.All.SendAsync("onReadyMatchHub", hub.Id.ToString());
    }

    public async Task WriteHubUpdated(IMatchHub hub)
    {
        _logger.LogDebug($"Hub updated: {hub.Id}");

        var dto = new MatchHubDto(hub);

        await _context.Clients.All.SendAsync("onUpdateddMatchHub", dto);
    }
}
