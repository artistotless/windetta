using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface IUnHoldBalances : CorrelatedBy<Guid>, ICommand
{
    public FundsInfo Funds { get; set; }
    public IEnumerable<Guid> UsersIds { get; set; }
}