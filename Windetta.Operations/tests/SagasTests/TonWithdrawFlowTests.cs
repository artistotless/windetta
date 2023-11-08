using MassTransit.QuartzIntegration;
using Windetta.Common.Testing;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Operations.Sagas;
using Windetta.TonTxnsTests;
using Xunit.Abstractions;

namespace Windetta.OperationsTests.Sagas;

public class TonWithdrawFlowTests : IClassFixture<HarnessFixture>
{
    private readonly ITestHarness _harness;
    private readonly XUnitOutWrapper _output;

    public TonWithdrawFlowTests(HarnessFixture harnessFixture, ITestOutputHelper output)
    {
        _harness = harnessFixture.Harness;
        _output = new XUnitOutWrapper(output);
    }

    [Fact]
    public async Task InitEventCausesSaveSagaData()
    {
        // arrange
        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000L
        };

        var stateMachineHarness = _harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        var stateMachine = stateMachineHarness.StateMachine;

        // act
        await _harness.Bus.Publish<IWithdrawTonRequested>(argument);

        // assert
        (await _harness.Consumed.Any<IWithdrawTonRequested>()).ShouldBe(true);
        (await stateMachineHarness.Consumed.Any<IWithdrawTonRequested>()).ShouldBe(true);

        var instance = stateMachineHarness.Created
             .ContainsInState(argument.CorrelationId, stateMachine,
             stateMachine.AwaitingDeduction);

        instance.ShouldNotBeNull();
        new QuartzTimeAdjustment()
        await _harness.OutputTimeline(_output, x => x.Now()); ;
    }

    [Fact]
    public async Task BalanceDeductedEvent()
    {
        // arrange
        var argument = new
        {
            UserId = Guid.NewGuid(),
            Destination = new TonAddress("EQCBvjU7mYLJQCIEtJGOiUWxmW0NI1Gn-1zzyTJ5zRBtLoLV"),
            CorrelationId = Guid.NewGuid(),
            Nanotons = 1_000_000_000L
        };

        var stateMachineHarness = _harness.GetSagaStateMachineHarness
            <TonWithdrawFlowStateMachine, TonWithdrawFlow>();
        var stateMachine = stateMachineHarness.StateMachine;

        await _harness.Bus.Publish<IWithdrawTonRequested>(argument);
        await _harness.Consumed.Any<IWithdrawTonRequested>();

        (await _harness.Sent.Any<IDeductBalance>()).ShouldBeTrue();

        // act
        await _harness.Bus.Publish<IBalanceDeducted>(new BalanceDeductedImpl()
        {
            CorrelationId = argument.CorrelationId
        });

        // assert
        await stateMachineHarness.Consumed.Any<IBalanceDeducted>();
        (await _harness.Sent.Any<ISendTons>()).ShouldBeTrue();
        (await stateMachineHarness.Exists(argument.CorrelationId,
             stateMachine.BalanceDeductedSuccess)).ShouldNotBeNull();
    }
}

public class BalanceDeductedImpl : IBalanceDeducted
{
    public Guid CorrelationId { get; set; }
}