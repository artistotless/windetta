using MassTransit;
using Windetta.Common.Types;

namespace Windetta.Operations.Sagas;

public class MatchFlow : SagaStateMachineInstance
{
    public Guid UserId { get; set; }
    public ulong Nanotons { get; set; }
    public TonAddress Destination { get; set; }
    public string CurrentState { get; set; }
    public Guid CorrelationId { get; set; }
    public string? FailReason { get; set; }
    public Guid? ExpirationTokenId { get; set; }
}

public class MatchFlowStateMachine : MassTransitStateMachine<MatchFlow>
{
    public MatchFlowStateMachine()
    {
        
    }
}