using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface INotifyMatchCanceled : CorrelatedBy<Guid>, ICommand
{
    public string Reason { get; set; }
}
