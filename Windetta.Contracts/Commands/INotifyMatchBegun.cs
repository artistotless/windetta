using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface INotifyMatchBegun : CorrelatedBy<Guid>, ICommand
{
    public Uri Endpoint { get; set; }
    public IReadOnlyDictionary<Guid, string> Tickets { get; set; }
}