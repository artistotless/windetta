using MassTransit;
using Polly;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Events;
using Windetta.TonTxns.Application.Services;

namespace Windetta.TonTxns.Infrastructure.HostedServices;

public class DepositPollerService : BackgroundService
{
    private readonly ILogger<DepositPollerService> _logger;
    private readonly DepositPoller _poller;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpoint;


    private int _executionCount;
    private const int _executionPeriodSeconds = 10;

    public DepositPollerService(
        ILogger<DepositPollerService> logger,
        DepositPoller poller,
        IPublishEndpoint publishEndpoint,
        ISendEndpointProvider sendEndpoint
        )
    {
        _logger = logger;
        _poller = poller;
        _publishEndpoint = publishEndpoint;
        _sendEndpoint = sendEndpoint;
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

    private async Task PollAsync()
    {
        var result = await _poller.ProcessAsync();

        var batchEvents = result.Values.Select(x => new FundsAddedImpl()
        {
            UserId = x.UserId,
            Funds = new FundsInfo((int)Currency.Ton, x.Amount),
            CorrelationId = x.Id,
        });

        var increaseCommand = new IncreaseBalanceImpl()
        {
            Data = result.Values.Select(x => new BalanceOperationData()
            {
                Funds = new FundsInfo((int)Currency.Ton, x.Amount),
                OperationId = x.Id,
                UserId = x.UserId
            }),
            CorrelationId = Guid.NewGuid(),
            Type = PositiveBalanceOperationType.TopUp,
        };

        if (batchEvents.Any())
        {
            await _publishEndpoint.PublishBatch<IFundsAdded>(batchEvents);

            var walletEndpoint = MyEndpointNameFormatter.CommandUri<IIncreaseBalance>(Svc.Wallet);
            var sendEndpoint = await _sendEndpoint.GetSendEndpoint(walletEndpoint);
            await sendEndpoint.Send(increaseCommand);

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
    public FundsInfo Funds { get; set; }
    public Guid CorrelationId { get; set; }
}

public class IncreaseBalanceImpl : IIncreaseBalance
{
    public Guid CorrelationId { get; set; }
    public PositiveBalanceOperationType Type { get; set; }
    public IEnumerable<BalanceOperationData> Data { get; set; }
}