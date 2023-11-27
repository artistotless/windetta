using Windetta.Main.MatchHub;

namespace Windetta.Main.Infrastructure.Data;

public class FakeMatchHubDispatcherOutputChannel : IMatchHubDispatcherOutputChannel
{
    private readonly ILogger<FakeMatchHubDispatcherOutputChannel> _logger;

    public FakeMatchHubDispatcherOutputChannel(ILogger<FakeMatchHubDispatcherOutputChannel> logger)
    {
        _logger = logger;
    }

    public Task WriteHubDeleted(IMatchHub hub)
    {
        _logger.LogDebug($"Hub deleted: {hub.Id}");

        return Task.CompletedTask;
    }

    public Task WriteHubReady(IMatchHub hub)
    {
        _logger.LogDebug($"Hub ready: {hub.Id}");

        return Task.CompletedTask;
    }

    public Task WriteHubUpdated(IMatchHub hub)
    {
        _logger.LogDebug($"Hub updated: {hub.Id}");

        return Task.CompletedTask;
    }
}
