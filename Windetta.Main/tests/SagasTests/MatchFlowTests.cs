using LSPM.Models;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Testing;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Infrastructure.Sagas;
using Windetta.MainTests.Mocks;
using Xunit.Abstractions;

namespace Windetta.MainTests.SagasTests;

public class MatchFlowTests : IUseHarness
{
    private readonly XUnitOutWrapper _output;

    private readonly Guid _correllationId;
    private const string _correllationIdString = "3bac654b-64db-4914-8d8e-b531ae796d5c";

    public MatchFlowTests(ITestOutputHelper output)
    {
        _output = new XUnitOutWrapper(output);
        _correllationId = Guid.Parse(_correllationIdString);
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
            CorrelationId = _correllationId,
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
            cfg.AddRequestClient<IMatchInfoRequested>();
            cfg.AddSagaStateMachine<MatchFlowStateMachine, MatchFlow>();
        };
    }
    #endregion

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
            CorrelationId = _correllationId,
            Reason = string.Empty,
        });
        await sagaHarness.Consumed.Any<ICancellationMatchRequested>();

        // assert
        (await sagaHarness.NotExists(_correllationId))
            .HasValue.ShouldBeFalse();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_MatchCreated()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = await provider.StartTestHarness();

        await provider.AddOrUpdateSaga(CreateSagaWithState
            (MatchFlowState.CreatingMatch));

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IMatchCreated>(new
        {
            CorrelationId = _correllationId,
        });

        await sagaHarness.Consumed.Any<IMatchCreated>();

        // assert
        (await harness.Sent.Any<INotifyReadyToConnect>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(_correllationId, s => s.Running))
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
            CorrelationId = _correllationId,
        });
        await sagaHarness.Consumed.Any<IMatchCompleted>();

        // assert
        (await sagaHarness.Exists(_correllationId, s => s.Final))
            .HasValue.ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_MatchInfoRequested()
    {
        // arrange
        await using var provider = GetProvider();

        var harness = await provider.StartTestHarness();
        var flow = CreateSagaWithState(initialState: MatchFlowState.Running);

        await provider.AddOrUpdateSaga(flow);

        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        var requestClient = provider.GetRequiredService<IRequestClient<IMatchInfoRequested>>();

        // act
        var response = await requestClient.GetResponse<MatchInfoResponse>(
            new { CorrelationId = _correllationId },
            CancellationToken.None,
            RequestTimeout.None
        );

        // assert
        response.Message.Bet.CurrencyId.ShouldBe(flow.BetCurrencyId);
        response.Message.Bet.Amount.ShouldBe(flow.BetAmount);
        response.Message.MatchId.ShouldBe(flow.CorrelationId);
        response.Message.Players.ToArray().ShouldBeEquivalentTo(flow.Players.ToArray());

        await harness.OutputTimeline(_output, x => x.Now());
    }
}