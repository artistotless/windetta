using LSPM.Models;
using MassTransit;
using Polly.Registry;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Services.LSPM;

namespace Windetta.Main.Infrastructure.Consumers;

public class SearchGameServerConsumer : IConsumer<ISearchGameServer>
{
    private readonly ILspms lspms;
    private readonly IRequestClient<IGameServerRequested> client;
    private readonly ResiliencePipelineProvider<Type>? retryPolicy;

    public SearchGameServerConsumer(
        ILspms lspms,
        IRequestClient<IGameServerRequested> client,
        ResiliencePipelineProvider<Type>? retryPolicy = null)
    {
        this.lspms = lspms;
        this.client = client;
        this.retryPolicy = retryPolicy;
    }

    public async Task Consume(ConsumeContext<ISearchGameServer> context)
    {
        var allLspms = await lspms.GetAllAsync();
        var pipeline = retryPolicy?.GetPipeline
            (typeof(SearchGameServerConsumer));

        RequestingGameServerResult result;

        if (pipeline is null)
            result = await RequestGameServer(context.Message, allLspms);
        else
            result = await pipeline.ExecuteAsync(
           async token => await RequestGameServer(context.Message, allLspms));

        if (result.Success)
        {
            await context.Publish<IGameServerFound>(new
            {
                context.Message.CorrelationId,
                result.Details!.Endpoint,
                result.Details.Tickets,
            });
        }
        else
        {
            await context.Publish<IGameServerReservationPeriodExpired>(new
            {
                context.Message.CorrelationId,
            });
        }
    }

    private async Task<RequestingGameServerResult> RequestGameServer
       (ISearchGameServer message, IEnumerable<Lspm> allLspms)
    {
        if (allLspms is null || allLspms.Count() == 0)
            throw LspmException.NotFound;

        RequestingGameServerResult? result = null;

        var request = new GameServerRequested()
        {
            CorrelationId = message.CorrelationId,
            GameId = message.GameId,
            Players = message.Players,
            Properties = message.Properties,
        };

        foreach (var item in allLspms)
        {
            request.LspmIp = item.Endpoint.DnsSafeHost;

            var response = await SendDurableRequest(request);

            if (response is not null && response.Message.Success)
            {
                result = response.Message;
                break;
            }
        }

        return result ?? throw LspmException.Overload;
    }

    private async Task<Response<RequestingGameServerResult>?> SendDurableRequest
        (IGameServerRequested request)
    {
        var requestTimeoutSeconds = 10;

        Action<SendContext<IGameServerRequested>> requestExpirationHeader = (context) =>
        {
            context.Headers.Set("expires",
                DateTimeOffset.UtcNow.AddSeconds(requestTimeoutSeconds));
        };

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

    private class GameServerRequested : IGameServerRequested
    {
        public Guid CorrelationId { get; set; }
        public Guid GameId { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public Dictionary<string, string>? Properties { get; set; }
        public string LspmIp { get; set; }
    }
}
