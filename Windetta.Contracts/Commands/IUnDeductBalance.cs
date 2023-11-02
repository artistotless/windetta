using MassTransit;

namespace Windetta.Contracts.Commands;

public interface IUnDeductBalance : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public long Amount { get; set; }
}