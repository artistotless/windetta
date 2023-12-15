using MassTransit;
using Polly.Registry;
using Windetta.Common.Helpers;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Services.LSPM;
using Windetta.Operations.Sagas;

namespace Windetta.Main.Infrastructure.Consumers;
public class SearchingGameServerConsumer : IConsumer<StartSearchingGameServer>
{
    private readonly ILspms lspms;
    private readonly IRequestClient<RequestGameServer> client;
    private readonly ResiliencePipelineProvider<Type>? retryPolicy;

    public SearchingGameServerConsumer(
        ILspms lspms,
        IRequestClient<RequestGameServer> client,
        ResiliencePipelineProvider<Type>? retryPolicy = null)
    {
        this.lspms = lspms;
        this.client = client;
        this.retryPolicy = retryPolicy;
    }

    public async Task Consume(ConsumeContext<StartSearchingGameServer> ctx)
    {
        var allLspms = await lspms.GetAllAsync();
        var pipeline = retryPolicy?.GetPipeline(typeof(MatchFlow));

        RequestingGameServerResult result;

        if (pipeline is null)
            result = await TryRequestGameServer(ctx, allLspms);
        else
            result = await pipeline.ExecuteAsync(
           async token => await TryRequestGameServer(ctx, allLspms));

        if (result.Success)
        {
            if (!result.IsCompletedResponse)
                return;

            await ctx.Publish<IGameServerPrepared>(new
            {
                ctx.Message.CorrelationId,
                result.Info!.Endpoint,
                result.Info.Tickets,
            });
        }
        else
        {
            await ctx.Publish<IGameServerReservationPeriodExpired>(new
            {
                ctx.Message.CorrelationId,
            });
        }
    }

    private async Task<RequestingGameServerResult> TryRequestGameServer(
        ConsumeContext<StartSearchingGameServer> ctx,
        IEnumerable<Lspm> allLspms)
    {
        RequestingGameServerResult? result = null;

        var requestTimeoutSeconds = 20;

        Action<SendContext<RequestGameServer>> requestExpirationHeader = (context) =>
        {
            context.Headers.Set("expires",
                DateTimeOffset.UtcNow.AddSeconds(requestTimeoutSeconds));
        };

        var request = new RequestGameServer()
        {
            CorrelationId = ctx.Message.CorrelationId,
            GameId = ctx.Message.GameId,
            Players = ctx.Message.Players
        };

        async Task<Response<RequestingGameServerResult>?> SendDurableRequest()
        {
            Response<RequestingGameServerResult> response;

            try
            {
                response = await client.GetResponse<RequestingGameServerResult>
                (request, x => x.UseExecute(requestExpirationHeader),
                timeout: RequestTimeout.After(s: requestTimeoutSeconds));
            }
            catch { return null; }

            return response;
        }

        foreach (var item in allLspms)
        {
            request.LspmKey = item.Endpoint.ParseIdentifier();

            var response = await SendDurableRequest();

            if (response is not null && response.Message.Success)
            {
                result = response.Message;
                break;
            }
        }

        return result ?? throw LspmException.NotFound;
    }
}

public class SearchingGameServerConsumerDefinition : ConsumerDefinition<SearchingGameServerConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<SearchingGameServerConsumer> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.PrefetchCount = 100;
        consumerConfigurator.Options<BatchOptions>(options => options
        .SetMessageLimit(100)
        .SetConcurrencyLimit(4)
        .SetTimeLimit(TimeSpan.FromSeconds(10)));

        consumerConfigurator.UseMessageRetry(r =>
        {
            r.Interval(retryCount: 5, interval: TimeSpan.FromSeconds(10));
        });
    }
}
