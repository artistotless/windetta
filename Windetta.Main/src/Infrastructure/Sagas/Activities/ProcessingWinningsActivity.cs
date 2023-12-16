using MassTransit;
using Windetta.Contracts.Events;

namespace Windetta.Main.Infrastructure.Sagas.Activities;

public class ProcessingWinningsActivity : IStateMachineActivity<MatchFlow, IMatchCompleted>
{
    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<MatchFlow, IMatchCompleted> context, IBehavior<MatchFlow, IMatchCompleted> next)
    {
        throw new Exception();

        await next.Execute(context);
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