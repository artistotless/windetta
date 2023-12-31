using LSPM.Models;
using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IGameServerRequested : CorrelatedBy<Guid>, IEvent
{
    public Guid CorrelationId { get; set; }
    public Guid GameId { get; set; }
    public IEnumerable<Player> Players { get; set; }
    public Dictionary<string, string> Properties { get; set; }
    public string LspmKey { get; set; }
}
