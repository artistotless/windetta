using MassTransit;
using Windetta.Contracts.Events;
using Windetta.Main.Infrastructure.Sagas;

namespace Windetta.MainTests.Mocks;

public class AlwaysFailsProcessingWinnings : IStateMachineActivity<MatchFlow, IMatchCompleted>
{
    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<MatchFlow, IMatchCompleted> context, IBehavior<MatchFlow, IMatchCompleted> next)
    {
        throw new Exception();
    }

    public Task Faulted<TException>(BehaviorExceptionContext<MatchFlow, IMatchCompleted, TException> context, IBehavior<MatchFlow, IMatchCompleted> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("processing-winnings");
    }
}