using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface INotifyServerFound : CorrelatedBy<Guid>, ICommand
{
    public Guid LobbyId { get; set; }
}