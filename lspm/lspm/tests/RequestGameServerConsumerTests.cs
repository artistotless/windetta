using LSPM.Infrastructure.Consumers;
using LSPM.Models;
using MassTransit;
using MassTransit.Testing;
using Windetta.Common.MassTransit;
using Windetta.Common.Testing;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;

namespace LspmTests;

public class RequestGameServerConsumerTests : IUseHarness
{
    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddRequestClient<IGameServerRequested>();
            cfg.AddConsumer<RequestGameServerConsumer>();
        };
    }

    [Fact]
    public async Task ConsumeEvent_Then_Hit_IServersManager()
    {
        // arrange
        var correlationId = Guid.NewGuid();
        var @event = new
        {
            TimeStamp = DateTimeOffset.UtcNow,
            CorrelationId = correlationId,
            GameId = ExampleGuids.GameId!,
            LspmKey = string.Empty,
            Players = new[] {
                new Player(Guid.NewGuid(), "Nick", 0),
                new Player(Guid.NewGuid(), "John", 0),
            }
        };

        var lspmMock = new LocalServerProcessManagerMock().Mock;
        await using var provider = SharedServiceProvider.GetProvider(services =>
        {
            services.AddSingleton(lspmMock.Object);
            services.ConfigureTestMassTransit(nameof(LspmTests), this);
        });

        var client = provider.GetRequiredService<IRequestClient<IGameServerRequested>>();
        var harness = await provider.StartTestHarness();

        // act
        await client.GetResponse<GameServerResponse>(@event);
        await harness.Consumed.Any<IGameServerRequested>();

        // assert
        lspmMock.Verify(x => x.GetOrLaunchGameServer(@event.GameId), Times.Once);
    }
}
