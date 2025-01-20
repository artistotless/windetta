using LSPM.Core.Interfaces;
using MassTransit;
using Serilog;
using Windetta.Contracts.Events;

namespace LSPM.Infrastructure.Services;

public sealed class InstanceLifetimeWatcher : BackgroundService
{
    private readonly IGameServersStore _servers;
    private readonly IServiceScopeFactory _scopeFactory;

    public InstanceLifetimeWatcher(IGameServersStore servers, IServiceScopeFactory scopeFactory)
    {
        _servers = servers;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(5));

        bool optimizedWay = false;

        if (_servers is InMemoryGameServersStore inMemoryStore)
            optimizedWay = true;

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                if (optimizedWay)
                    await IterateOnPair();
                else
                    await IterateOnCollection();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
    }

    private async Task IterateOnCollection()
    {
        var entries = _servers.GetAll();

        foreach (var item in entries)
        {
            if (DateTimeOffset.UtcNow.Subtract(item.LastPing) > TimeSpan.FromSeconds(60))
            {
                await NotifyContainingMatchesCanceled(item.InstanceId, item.Matches);
                Log.ForContext<InstanceLifetimeWatcher>().Information(
                    "Match canceled! The game server '{gameServerHost}' didn't respond for a long time",
                    item.Endpoint.Host);
            }
        }
    }

    private async Task IterateOnPair()
    {
        var enumerator = (_servers as InMemoryGameServersStore)!;

        foreach (var item in enumerator)
        {
            if (DateTimeOffset.UtcNow.Subtract(item.Value.LastPing) > TimeSpan.FromSeconds(60))
            {
                await NotifyContainingMatchesCanceled(item.Value.InstanceId, item.Value.Matches);
                Log.ForContext<InstanceLifetimeWatcher>().Information(
                    "Match canceled! The game server '{gameServerHost}' didn't respond for a long time",
                    item.Value.Endpoint.Host);
            }
        }
    }

    private async Task NotifyContainingMatchesCanceled
        (Guid instanceId, IReadOnlyCollection<Guid>? matches)
    {
        _servers.Remove(instanceId);

        if (matches is null)
            return;

        var batch = BuildCancellationEventBatch(matches, "gameserver_crashed");
        using var scope = _scopeFactory.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        await publisher.PublishBatch(batch);
    }

    private static IEnumerable<ICancellationMatchRequested> BuildCancellationEventBatch
    (IEnumerable<Guid> matchesId, string reason)
    {
        foreach (var id in matchesId)
        {
            yield return new CancellationMatchRequested()
            {
                CorrelationId = id,
                Reason = reason
            };
        }
    }

    private class CancellationMatchRequested : ICancellationMatchRequested
    {
        public string Reason { get; set; }
        public Guid CorrelationId { get; set; }
    }
}
