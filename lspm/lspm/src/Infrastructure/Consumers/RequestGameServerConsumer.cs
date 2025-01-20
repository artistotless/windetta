using LSPM.Core.Interfaces;
using LSPM.Core.Models;
using MassTransit;
using Serilog;
using Windetta.Common.MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;

namespace LSPM.Infrastructure.Consumers;

[ExcludeFromAutoRegisterConsumer]
public class RequestGameServerConsumer : IConsumer<IGameServerRequested>
{
    private readonly ILocalServerProcessManager _lspm;

    public RequestGameServerConsumer(ILocalServerProcessManager lspm)
    {
        _lspm = lspm;
    }

    public async Task Consume(ConsumeContext<IGameServerRequested> context)
    {
        var gameId = context.Message.GameId;
        var expiresTimeSeconds = context.GetHeader<int>("expires") ?? 10;
        var cancellationDelay = DateTimeOffset.Now
            .Subtract(context.Message.TimeStamp
            .ToLocalTime()
            .AddSeconds(expiresTimeSeconds / 2))
            .TotalMilliseconds;

        cancellationDelay = Math.Abs(cancellationDelay);

        try
        {
            var cancellation = new CancellationTokenSource((int)cancellationDelay);

            GameServerInfo serverInfo = await _lspm.GetOrLaunchGameServer(gameId);

            Log.ForContext<RequestGameServerConsumer>().Information
                ("Game server started: {gameServerId}", serverInfo.InstanceId);

            await context.RespondAsync(new GameServerResponse()
            {
                GameServerEndpoint = serverInfo.Endpoint,
                GameServerId = serverInfo.InstanceId,
            });
        }
        catch (WindettaException e)
        {
            await context.RespondAsync(new GameServerResponse()
            {
                Error = e.Message,
            });
        }
    }
}
