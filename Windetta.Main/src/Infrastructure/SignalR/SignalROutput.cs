using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.Dtos;
using Windetta.Main.Infrastructure.SignalR;

namespace Windetta.Main.Infrastructure.MatchHub;

internal class SignalROutput : IMatchHubObserverOutput
{
    private readonly ILogger<SignalROutput> _logger;
    private readonly IHubContext<MainHub> _context;

    public SignalROutput(ILogger<SignalROutput> logger, IHubContext<MainHub> context)
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

        var dto = hub is TournamentMatchHub ?
            new TournamentMatchHubDto(hub) :
            new MatchHubDto(hub);

        await _context.Clients.All.SendAsync("onUpdateddMatchHub", dto);
    }
}
