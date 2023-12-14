using MassTransit;
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
using Windetta.MainTests.Mocks;
using Windetta.Operations.Sagas;
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

    private ServiceProvider GetProvider()
    {
        var storageMock = new MatchHubsMock()
        {
            ReturnThisHub = new ProxyMatchHub(new MatchHubOptions()
            {
                InitiatorId = IdExamples.UserId,
                GameConfiguration = new GameConfiguration() { MaxPlayers = 2 },
            })
        };

        return SharedServiceProvider.GetInstance(services =>
        {
            services.AddSingleton(p => storageMock.Mock.Object);
            services.AddScoped(p => new LspmsMock().Mock.Object);
            services.ConfigureMassTransit(Svc.Main, this);
        });
    }

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddRequestClient<RequestGameServer>();
            cfg.AddSagaStateMachine<MatchFlowStateMachine, MatchFlow>();
        };
    }

    [Fact]
    public async Task When_MatchHubReady()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = provider.GetTestHarness();
        var argument = new
        {
            CorrelationId = correllationId,
            TimeStamp = DateTimeOffset.UtcNow
        };

        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IMatchHubReady>(argument);


        // assert
        (await sagaHarness.Consumed.Any<IMatchHubReady>())
            .ShouldBeTrue();
        (await sagaHarness.Exists(argument.CorrelationId, x => x.GameServerSearch))
            .HasValue.ShouldBeTrue();
        (await harness.Sent.Any<StartSearchingGameServer>())
            .ShouldBeTrue();

        await harness.OutputTimeline(_output, x => x.Now());
    }

    [Fact]
    public async Task When_GameServerPrepared()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = provider.GetTestHarness();
        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();
        await harness.Bus.Publish<IMatchHubReady>(new
        {
            CorrelationId = correllationId,
            TimeStamp = DateTimeOffset.UtcNow
        });
        await sagaHarness.Consumed.Any<IMatchHubReady>();

        // act
        await harness.Bus.Publish<IGameServerPrepared>(new
        {
            CorrelationId = correllationId,
            Endpoint = "gameserver-1",
            Tickets = new Dictionary<Guid, string>() { { Guid.Empty, "ticket1" } }.AsReadOnly()
        });
        await sagaHarness.Consumed.Any<IGameServerPrepared>();

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
        var harness = provider.GetTestHarness();
        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();
        await harness.Bus.Publish<IMatchHubReady>(new
        {
            CorrelationId = correllationId,
            TimeStamp = DateTimeOffset.UtcNow
        });
        await sagaHarness.Consumed.Any<IMatchHubReady>();

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
        var harness = provider.GetTestHarness();
        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();
        await harness.Bus.Publish<IMatchHubReady>(new
        {
            CorrelationId = correllationId,
            TimeStamp = DateTimeOffset.UtcNow
        });
        await sagaHarness.Consumed.Any<IMatchHubReady>();

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
        var harness = provider.GetTestHarness();
        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();
        await harness.Bus.Publish<IMatchHubReady>(new
        {
            CorrelationId = correllationId,
            TimeStamp = DateTimeOffset.UtcNow
        });
        await sagaHarness.Consumed.Any<IMatchHubReady>();
        await harness.Bus.Publish<IGameServerPrepared>(new
        {
            CorrelationId = correllationId,
            Endpoint = "gameserver-1",
            Tickets = new Dictionary<Guid, string>() { { Guid.Empty, "ticket1" } }.AsReadOnly()
        });
        await sagaHarness.Consumed.Any<IGameServerPrepared>();

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
    public async Task When_MatchCompleted()
    {
        // arrange
        await using var provider = GetProvider();
        var harness = provider.GetTestHarness();
        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();
        await harness.Bus.Publish<IMatchHubReady>(new
        {
            CorrelationId = correllationId,
            TimeStamp = DateTimeOffset.UtcNow
        });
        await sagaHarness.Consumed.Any<IMatchHubReady>();
        await harness.Bus.Publish<IGameServerPrepared>(new
        {
            CorrelationId = correllationId,
            Endpoint = "gameserver-1",
            Tickets = new Dictionary<Guid, string>() { { Guid.Empty, "ticket1" } }.AsReadOnly()
        });
        await sagaHarness.Consumed.Any<IGameServerPrepared>();

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
