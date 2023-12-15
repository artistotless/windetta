using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface INotifyMatchBegun : CorrelatedBy<Guid>, ICommand
{
    public string Endpoint { get; set; }
    public IReadOnlyDictionary<Guid, string> Tickets { get; set; }
}