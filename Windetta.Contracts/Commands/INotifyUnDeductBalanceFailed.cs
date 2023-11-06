using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface INotifyUnDeductBalanceFailed : CorrelatedBy<Guid>, ICommand
{
    public Guid UserId { get; set; }
}