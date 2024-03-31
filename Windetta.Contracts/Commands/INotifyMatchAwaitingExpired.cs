using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface INotifyMatchAwaitingExpired : CorrelatedBy<Guid>, ICommand
{
    public Guid LobbyId { get; set; }
}