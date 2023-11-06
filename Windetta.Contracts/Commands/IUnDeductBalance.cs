using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface IUnDeductBalance : CorrelatedBy<Guid>, ICommand
{
    public Guid UserId { get; set; }
    public long Amount { get; set; }
}