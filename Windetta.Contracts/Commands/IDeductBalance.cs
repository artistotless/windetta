using MassTransit;

namespace Windetta.Contracts.Commands;

public interface IDeductBalance : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public long Amount { get; set; }
}