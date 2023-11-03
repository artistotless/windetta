using MassTransit;

namespace Windetta.Contracts.Commands;

public interface INotifyUnDeductBalanceFailed : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
}