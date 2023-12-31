using LSPM.Models;
using MassTransit;
using Polly.Registry;
using Windetta.Common.Helpers;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Services.LSPM;


namespace Windetta.Main.Infrastructure.Sagas.Activities;

public class SearchingGameServerActivity : IStateMachineActivity<MatchFlow>
{
    private readonly ILspms lspms;
    private readonly IRequestClient<IGameServerRequested> client;
    private readonly ResiliencePipelineProvider<Type>? retryPolicy;

    public SearchingGameServerActivity(
        ILspms lspms,
        IRequestClient<IGameServerRequested> client,
        ResiliencePipelineProvider<Type>? retryPolicy = null)
    {
        this.lspms = lspms;
        this.client = client;
        this.retryPolicy = retryPolicy;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<MatchFlow> context, IBehavior<MatchFlow> next)
    {
        var allLspms = await lspms.GetAllAsync();
        var pipeline = retryPolicy?.GetPipeline(typeof(MatchFlow));

        RequestingGameServerResult result;

        if (pipeline is null)
            result = await TryRequestGameServer(context, allLspms);
        else
            result = await pipeline.ExecuteAsync(
           async token => await TryRequestGameServer(context, allLspms));

        if (result.Success)
        {
            await context.Publish<IGameServerFound>(new
            {
                context.Saga.CorrelationId,
                result.Details!.Endpoint,
                result.Details.Tickets,
            });
        }
        else
        {
            await context.Publish<IGameServerReservationPeriodExpired>(new
            {
                context.Saga.CorrelationId,
            });
        }

        await next.Execute(context).ConfigureAwait(false);
    }

    private async Task<RequestingGameServerResult> TryRequestGameServer
        (BehaviorContext<MatchFlow> ctx, IEnumerable<Lspm> allLspms)
    {
        if (allLspms is null || allLspms.Count() == 0)
            throw LspmException.NotFound;

        RequestingGameServerResult? result = null;

        var requestTimeoutSeconds = 20;

        Action<SendContext<IGameServerRequested>> requestExpirationHeader = (context) =>
        {
            context.Headers.Set("expires",
                DateTimeOffset.UtcNow.AddSeconds(requestTimeoutSeconds));
        };

        var request = new GameServerRequested()
        {
            CorrelationId = ctx.Saga.CorrelationId,
            GameId = ctx.Saga.GameId,
            Players = ctx.Saga.Players,
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

        return result ?? throw LspmException.Overload;
    }

    public Task Execute<T>(BehaviorContext<MatchFlow, T> context, IBehavior<MatchFlow, T> next) where T : class
    {
        return next.Execute(context);
    }


    public Task Faulted<TException>(BehaviorExceptionContext<MatchFlow, TException> context, IBehavior<MatchFlow> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public Task Faulted<T, TException>(BehaviorExceptionContext<MatchFlow, T, TException> context, IBehavior<MatchFlow, T> next)
        where T : class
        where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("hold-balances");
    }
    private class GameServerRequested : IGameServerRequested
    {
        public Guid CorrelationId { get; set; }
        public Guid GameId { get; set; }
        public IEnumerable<Player> Players { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public string LspmKey { get; set; }
    }
}


