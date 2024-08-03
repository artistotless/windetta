using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface INotifyMatchCanceled : CorrelatedBy<Guid>, ICommand
{
    public IEnumerable<Guid> UsersIds { get; set; }
    public string Reason { get; set; }

}
