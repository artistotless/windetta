using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface IDeductUnHoldBalance : ICommand, CorrelatedBy<Guid>
{
    public NegativeBalanceOperationType Type { get; set; }
    public IEnumerable<BalanceOperationData> Data { get; set; }
}