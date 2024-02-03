using LSPM.Models;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Testing;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Services.LSPM;
using Windetta.Main.Infrastructure.Sagas;
using Windetta.MainTests.Mocks;
using Xunit.Abstractions;

namespace Windetta.MainTests.SagasTests;

public class MatchFlowTests : IUseHarness
{
    private readonly XUnitOutWrapper _output;

    private readonly Guid correllationId;
    private const string correllationIdString = "3bac654b-64db-4914-8d8e-b531ae796d5c";

    public MatchFlowTests(ITestOutputHelper output)
    {
        _output = new XUnitOutWrapper(output);
        correllationId = Guid.Parse(correllationIdString);
    }

    #region Configuration
    private ServiceProvider GetProvider()
    {
        var storageMock = new LobbiesMock()
        {
            ReturnThisLobby = new ProxyLobby(new LobbyOptions()
            {
                InitiatorId = ExampleGuids.UserId,
                GameConfiguration = new GameConfiguration(2, 2),
            })
        };

        return SharedServiceProvider.GetInstance(services =>
        {
            services.AddSingleton(p => storageMock.Mock.Object);
            services.AddScoped(p => new LspmsMock().Mock.Object);
            services.ConfigureTestMassTransit(Svc.Main, this);
        });
    }

    private MatchFlow CreateSagaWithState(MatchFlowState initialState)
    {
        return new MatchFlow
        {
            CurrentState = (int)initialState,
            CorrelationId = correllationId,
            BetAmount = 1000,
            BetCurrencyId = 1,
            Created = DateTimeOffset.UtcNow,
            GameId = ExampleGuids.GameId,
            Players = new[] { new Player(Guid.NewGuid(), "Nick", 0) }
        };
    }

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddRequestClient<IGameServerRequested>();
            cfg.AddSagaStateMachine<MatchFlowStateMachine, MatchFlow>();
        };
    }
    #endregion

    [Fact]
    public async Task When_LobbyReady()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();
        var argument = new
        {
            CorrelationId = correllationId,
            TimeStamp = DateTimeOffset.UtcNow
        };
        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<ILobbyReady>(argument);

        // assert
        (await sagaHarness.Consumed.Any<ILobbyReady>())
                .ShouldBeTrue();
        (await sagaHarness.Exists(argument.CorrelationId, x => x.AwaitingHoldBalances))
                .HasValue.ShouldBeTrue();
        (await harness.Sent.Any<IHoldBalances>())
            .ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_BalancesHeld_LspmResponds()
    {
        // arrange
        var lspm = new Lspm()
        {
            Endpoint = new Uri("http://127.0.0.1:3303"),
            Id = Guid.NewGuid(),
            Load = 0,
            SupportedGames = new[] { ExampleGuids.GameId }
        };

        await using var provider = SharedServiceProvider.GetInstance(services =>
        {
            services.RegisterConsumer<AlwaysRespondsSuccessLspmConsumer>();
            services.RegisterConsumer<TestSearchGameServerConsumer>();
            services.AddScoped(p => new LspmsMock(new List<Lspm> { lspm }).Mock.Object);
            services.ConfigureTestMassTransit(Svc.Main, this);
        });

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.AwaitingHoldBalances));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IBalancesHeld>(new
        {
            CorrelationId = correllationId,
        });
        await sagaHarness.Consumed.Any<IBalancesHeld>();

        // assert
        (await harness.Consumed.Any<ISearchGameServer>())
            .ShouldBeTrue();
        (await harness.Published.Any<IGameServerRequested>())
            .ShouldBeTrue();
        (await sagaHarness.Consumed.Any<IGameServerFound>())
            .ShouldBeTrue();
        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_BalancesHeld_LspmOverload()
    {
        // arrange
        var lspm = new Lspm()
        {
            Endpoint = new Uri("http://127.0.0.1:3303"),
            Id = Guid.NewGuid(),
            Load = 0,
            SupportedGames = new[] { ExampleGuids.GameId }
        };

        await using var provider = SharedServiceProvider.GetInstance(services =>
        {
            services.RegisterConsumer<AlwaysOverloadLspmConsumer>();
            services.RegisterConsumer<TestSearchGameServerConsumer>();
            services.AddScoped(p => new LspmsMock(new List<Lspm> { lspm }).Mock.Object);
            services.ConfigureTestMassTransit(Svc.Main, this);
        });

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.AwaitingHoldBalances));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IBalancesHeld>(new
        {
            CorrelationId = correllationId,
        });
        await sagaHarness.Consumed.Any<IBalancesHeld>();

        // assert
        (await harness.Published.Any<IGameServerRequested>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(correllationId, s => s.GameServerSearching))
            .HasValue.ShouldBeTrue();
        (await harness.Published.Any<Fault<ISearchGameServer>>())
            .ShouldBeTrue();
        (await harness.Sent.Any<INotifyMatchAwaitingExpired>())
              .ShouldBeTrue();
        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_GameServerFound()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.GameServerSearching));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IGameServerFound>(new
        {
            CorrelationId = correllationId,
            Endpoint = new Uri("https://127.0.0.0:9090"),
            Tickets = new[] {
                new Ticket(){PlayerId = Guid.NewGuid(), Value="ffff" },
                new Ticket(){PlayerId = Guid.NewGuid(), Value="ffff" },
            }
        });
        await sagaHarness.Consumed.Any<IGameServerFound>();

        // assert
        (await harness.Sent.Any<INotifyServerFound>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(correllationId, s => s.ServerFound))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_ReadyAcceptConnections()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.ServerFound));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IGameServeReadyAcceptConnections>(new
        {
            CorrelationId = correllationId,
        });
        await sagaHarness.Consumed.Any<IGameServeReadyAcceptConnections>();

        // assert
        (await harness.Sent.Any<INotifyMatchBegun>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(correllationId, s => s.Running))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_GameServerReservationPeriodExpired()
    {
        // arrange
        await using var provider = SharedServiceProvider.GetInstance(services =>
        {
            services.RegisterConsumer<AlwaysFaultSearchGameServerConsumer>();
            services.ConfigureTestMassTransit(Svc.Main, this);
        });

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.GameServerSearching));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.SendCommandAsync<ISearchGameServer>(Svc.Main, new
        {
            GameId = Guid.NewGuid(),
            Players = new[] { new Player(), new Player(), },
            CorrelationId = correllationId,
        });

        await sagaHarness.Consumed.Any<ISearchGameServer>();

        // assert
        (await harness.Sent.Any<INotifyMatchAwaitingExpired>())
            .ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task During_GameServerSearch_When_CancellationRequested()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.GameServerSearching));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<ICancellationMatchRequested>(new
        {
            CorrelationId = correllationId,
        });
        await sagaHarness.Consumed.Any<ICancellationMatchRequested>();

        // assert
        (await sagaHarness.NotExists(correllationId))
            .HasValue.ShouldBeFalse();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task During_Running_When_CancellationRequested()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.Running));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<ICancellationMatchRequested>(new
        {
            CorrelationId = correllationId,
            Reason = string.Empty,
        });
        await sagaHarness.Consumed.Any<ICancellationMatchRequested>();

        // assert
        (await sagaHarness.NotExists(correllationId))
            .HasValue.ShouldBeFalse();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_MatchCompleted()
    {
        // arrange
        await using var provider = GetProvider();

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.Running));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IMatchCompleted>(new
        {
            CorrelationId = correllationId,
        });
        await sagaHarness.Consumed.Any<IMatchCompleted>();

        // assert
        (await sagaHarness.Exists(correllationId, s => s.ProcessingWinnings))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }
}