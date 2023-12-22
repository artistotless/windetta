using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface INotifyUnDeductBalanceFailed : CorrelatedBy<Guid>, ICommand
{
    public Guid UserId { get; set; }
}