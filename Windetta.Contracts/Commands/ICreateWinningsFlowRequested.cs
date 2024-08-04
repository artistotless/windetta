using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface ICreateWinningsFlowRequested : CorrelatedBy<Guid>, IEvent
{
    public IEnumerable<Guid> Winners { get; set; }
    public IEnumerable<Guid> Losers { get; set; }
    public FundsInfo Funds { get; set; }
}