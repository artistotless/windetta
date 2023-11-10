using MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxns.Infrastructure.HostedServices;

public class DepositPollerService : BackgroundService
{
    private readonly ILogger<DepositPollerService> _logger;
    private readonly DepositPoller _poller;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICurrencyIdProvider _currencyIdProvider;

    private int _executionCount;
    private const int _executionPeriodSeconds = 10;

    public DepositPollerService(
        ILogger<DepositPollerService> logger,
        IPublishEndpoint publishEndpoint,
        DepositPoller poller,
        ICurrencyIdProvider currencyIdProvider)
    {
        _logger = logger;
        _poller = poller;
        _publishEndpoint = publishEndpoint;
        _currencyIdProvider = currencyIdProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(DepositPollerService)} running.");

        // When the timer should have no due-time, then do the work once now.
        await PollAsync();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(_executionPeriodSeconds));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await PollAsync();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation($"{nameof(DepositPollerService)} is stopping.");
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
            CurrencyId = _currencyIdProvider.Id,
            CorrelationId = x.Id
        });

        await _publishEndpoint.PublishBatch<IFundsAdded>(batchEvents);

        int count = Interlocked.Increment(ref _executionCount);

        _logger.LogInformation($"{nameof(DepositPollerService)} is working. Count: {count}");
    }
}

public class FundsAddedImpl : IFundsAdded
{
    public Guid UserId { get; set; }
    public long Amount { get; set; }
    public int CurrencyId { get; set; }
    public Guid CorrelationId { get; set; }
}