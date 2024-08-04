using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IIncreaseBalance : ICommand, CorrelatedBy<Guid>
{
    public PositiveBalanceOperationType Type { get; set; }
    public IEnumerable<BalanceOperationData> Data { get; set; }
}
