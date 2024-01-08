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
using Windetta.Main.Core.MatchHubs;
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
        var storageMock = new MatchHubsMock()
        {
            ReturnThisHub = new ProxyMatchHub(new MatchHubOptions()
            {
                InitiatorId = ExampleGuids.UserId,
                GameConfiguration = new GameConfiguration(2,2),
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
    public async Task When_MatchHubReady()
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
        await harness.Bus.Publish<IMatchHubReady>(argument);

        // assert
        (await sagaHarness.Consumed.Any<IMatchHubReady>())
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
        (await sagaHarness.Exists(correllationId, s => s.GameServerSearch))
            .HasValue.ShouldBeTrue();
        (await harness.Sent.Any<INotifyMatchCanceled>())
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
            (MatchFlowState.GameServerSearch));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IGameServerFound>(new
        {
            CorrelationId = correllationId,
            Endpoint = "gameserver-1",
            Tickets = new Dictionary<Guid, string>() { { Guid.Empty, "ticket1" } }.AsReadOnly()
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
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.GameServerSearch));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IGameServerReservationPeriodExpired>(new
        {
            CorrelationId = correllationId,
        });
        await sagaHarness.Consumed.Any<IGameServerReservationPeriodExpired>();

        // assert
        (await harness.Sent.Any<INotifyMatchAwaitingExpired>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(correllationId, s => s.GameServerSearchExpired))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task During_GameServerSearch_When_CancellationRequested()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.GameServerSearch));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<ICancellationMatchRequested>(new
        {
            CorrelationId = correllationId,
        });
        await sagaHarness.Consumed.Any<ICancellationMatchRequested>();

        // assert
        (await sagaHarness.Exists(correllationId, s => s.Canceled))
            .HasValue.ShouldBeTrue();

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
        (await sagaHarness.Exists(correllationId, s => s.Canceled))
            .HasValue.ShouldBeTrue();

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

    [Fact]
    public async Task When_Finalized()
    {
        // arrange
        await using var provider = GetProvider();

        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.ProcessingWinnings));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IWinningsProcessed>(new
        {
            CorrelationId = correllationId,
        });
        await sagaHarness.Consumed.Any<IWinningsProcessed>();

        // assert
        await Task.Delay(2_000);
        //(await sagaHarness.Exists(correllationId, s => s.ProcessingWinnings))
        //    .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }
}