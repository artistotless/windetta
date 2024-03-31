using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface ISearchGameServer : CorrelatedBy<Guid>, ICommand
{
    public Guid GameId { get; set; }
}
