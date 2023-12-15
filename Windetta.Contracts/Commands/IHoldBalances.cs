using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface IHoldBalances : CorrelatedBy<Guid>, ICommand
{
    public FundsInfo Funds { get; set; }
    public IEnumerable<Guid> UsersIds { get; set; }
}