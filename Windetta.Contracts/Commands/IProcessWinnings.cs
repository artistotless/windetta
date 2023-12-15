using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface IProcessWinnings : CorrelatedBy<Guid>, ICommand
{
    public IEnumerable<Guid> Winners { get; set; }
    public FundsInfo Funds { get; set; }
}