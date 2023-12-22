using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface IProcessWinnings : CorrelatedBy<Guid>, ICommand
{
    public IEnumerable<Guid> Winners { get; set; }
    public FundsInfo Funds { get; set; }
}