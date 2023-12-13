using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Common.Testing;
using Windetta.Contracts.Events;
using Windetta.MainTests.Mocks;
using Windetta.Operations.Sagas;
using Xunit.Abstractions;

namespace Windetta.MainTests.SagasTests;

public class MatchFlowTests : IUseHarness
{
    private readonly XUnitOutWrapper _output;


    public MatchFlowTests(ITestOutputHelper output)
    {
        _output = new XUnitOutWrapper(output);
    }

    public Action<IBusRegistrationConfigurator> ConfigureHarness()
    {
        return cfg =>
        {
            cfg.AddSagaStateMachine<MatchFlowStateMachine, MatchFlow>();
        };
    }

    [Fact]
    public async Task MatchHubReadyEvent_ShouldCreateSagaInstance()
    {
        // arrange
        var options = new MatchHubOptions()
        {
            InitiatorId = IdExamples.UserId,
            GameConfiguration = new GameConfiguration() { MaxPlayers = 2 },
        };
        var hub = new ProxyMatchHub(options);
        var storageMock = new MatchHubsMock() { ReturnThisHub = hub }.Mock;

        await using var provider = SharedServiceProvider.GetInstance(services =>
        {
            services.AddSingleton(p => storageMock.Object);
            services.AddScoped(p => new LspmsMock().Mock.Object);
            services.ConfigureMassTransit(Svc.Main, this);
        });

        var harness = provider.GetTestHarness();
        var argument = new
        {
            CorrelationId = Guid.NewGuid(),
            TimeStamp = DateTimeOffset.UtcNow
        };

        await harness.Start();
        var sagaHarness = harness.GetSagaStateMachineHarness
            <MatchFlowStateMachine, MatchFlow>();

        // act
        await harness.Bus.Publish<IMatchHubReady>(argument);


        // assert
        await sagaHarness.Consumed.Any<IMatchHubReady>();
        await sagaHarness.Exists(argument.CorrelationId, x => x.ConnectiongToLspm);

        await harness.OutputTimeline(_output, x => x.Now());
    }
}
