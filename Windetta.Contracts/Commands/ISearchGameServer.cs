using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface ISearchGameServer : CorrelatedBy<Guid>, ICommand
{
    public Guid GameId { get; set; }
    public IEnumerable<Player> Players { get; set; }
    public Dictionary<string, string>? Properties { get; set; }
}
