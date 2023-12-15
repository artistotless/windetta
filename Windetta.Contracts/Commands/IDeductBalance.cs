using MassTransit;
using Windetta.Common.Messages;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface IDeductBalance : ICommand, CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public FundsInfo Funds { get; set; }
}