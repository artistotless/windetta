using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface IDeductBalance : ICommand, CorrelatedBy<Guid>
{
    public NegativeBalanceOperationType Type { get; set; }
    public Guid UserId { get; set; }
    public FundsInfo Funds { get; set; }
}
