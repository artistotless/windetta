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

    private const int REQUEST_TIMEOUT_SECONDS = 10;
    private string _current_lspm;

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

        GameServerResult result;

        if (pipeline is null)
            result = await RequestGameServer(context.Message, allLspms);
        else
            result = await pipeline.ExecuteAsync(
            async token => await RequestGameServer(context.Message, allLspms));

        await context.Publish<IGameServerFound>(new
        {
            context.Message.CorrelationId,
            result.GameServerId,
            result.GameServerEndpoint,
            LspmIp = _current_lspm,
        });
    }

    private async Task<GameServerResult> RequestGameServer
       (ISearchGameServer message, IEnumerable<Lspm> allLspms)
    {
        if (allLspms is null || allLspms.Count() == 0)
            throw LspmException.NotFound;

        GameServerResult? result = null;

        var request = new GameServerRequested()
        {
            CorrelationId = message.CorrelationId,
            GameId = message.GameId,
        };

        foreach (var item in allLspms)
        {
            _current_lspm = item.Endpoint.DnsSafeHost;
            request.LspmIp = _current_lspm;
            var response = await SendDurableRequest(request);

            if (response is not null && response.Success)
            {
                result = response;
                break;
            }
        }

        return result ?? throw LspmException.Overload;
    }

    private async Task<GameServerResult?> SendDurableRequest(IGameServerRequested request)
    {
        Action<SendContext<IGameServerRequested>> setExpirationHeader = (context) =>
        {
            context.Headers.Set("expires", REQUEST_TIMEOUT_SECONDS);
        };

        try
        {
            request.TimeStamp = DateTime.UtcNow;
            var response = await client.GetResponse<GameServerResult>
            (request, x => x.UseExecute(setExpirationHeader),
            timeout: RequestTimeout.After(s: REQUEST_TIMEOUT_SECONDS));

            return response.Message;
        }
        catch { return null; }
    }

    private class GameServerRequested : IGameServerRequested
    {
        public Guid CorrelationId { get; set; }
        public Guid GameId { get; set; }
        public string LspmIp { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
