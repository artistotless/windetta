using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Operations.Sagas;
using Windetta.TonTxnsTests;

namespace Windetta.OperationsTests.Sagas;

public class TonWithdrawFlowTests : IClassFixture<HarnessFixture>
{
    private readonly ITestHarness _harness;

    public TonWithdrawFlowTests(HarnessFixture harnessFixture)
    {
        _harness = harnessFixture.Harness;
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
    }
}
