using MassTransit;
using Polly;
using Windetta.Common.Constants;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxns.Infrastructure.HostedServices;

public class DepositPollerService : BackgroundService
{
    private readonly ILogger<DepositPollerService> _logger;
    private readonly DepositPoller _poller;
    private readonly IPublishEndpoint _publishEndpoint;

    private int _executionCount;
    private const int _executionPeriodSeconds = 10;

    public DepositPollerService(
        ILogger<DepositPollerService> logger,
        DepositPoller poller,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _poller = poller;
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Running.");

        // When the timer should have no due-time, then do the work once now.
        await TryPollAsync();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(_executionPeriodSeconds));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await TryPollAsync();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation($"Stopping.");
        }
    }

    private async Task TryPollAsync()
    {
        try
        {
            await GetRetryPipeline().ExecuteAsync(async (token) =>
            {
                await PollAsync();
            });
        }
        catch (Exception e)
        {

            _logger.LogError(e, $"Error when trying to receive deposits");
        }
    }

    // Could also be a async method, that can be awaited in ExecuteAsync above
    private async Task PollAsync()
    {
        var result = await _poller.ProcessAsync();

        var batchEvents = result.Values.Select(x => new FundsAddedImpl()
        {
            UserId = x.UserId,
            Amount = x.Amount,
            CurrencyId = (int)Currencies.Ton,
            CorrelationId = x.Id
        });

        if (batchEvents.Any())
        {
            await _publishEndpoint.PublishBatch<IFundsAdded>(batchEvents);
            _logger.LogInformation($"Handled new deposits: {batchEvents.Count()}");
        }

        int count = Interlocked.Increment(ref _executionCount);

        _logger.LogInformation($"Working. Requests Count: {count}");
    }

    private static ResiliencePipeline GetRetryPipeline()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new()
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2)
            })
            .Build();
    }
}

public class FundsAddedImpl : IFundsAdded
{
    public Guid UserId { get; set; }
    public ulong Amount { get; set; }
    public int CurrencyId { get; set; }
    public Guid CorrelationId { get; set; }
}