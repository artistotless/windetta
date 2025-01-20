using LSPM.Core.Services;
using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Events;

namespace LSPM.Infrastructure.Consumers;

public class CreateMatchConsumer : IConsumer<ICreateMatchRequested>
{
    private readonly GameServersFacadeService _gameServers;

    public CreateMatchConsumer(GameServersFacadeService facade)
    {
        _gameServers = facade;
    }

    public async Task Consume(ConsumeContext<ICreateMatchRequested> context)
    {
        var matchInitData = new MatchInitializationData()
        {
            MatchId = context.Message.CorrelationId,
            Players = context.Message.Players,
            Properties = context.Message.Properties,
        };

        await _gameServers.CreateMatchAsync(context.Message.GameServerId, matchInitData);
    }
}