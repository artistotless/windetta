using MassTransit;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Core.Services.LSPM;

public class RequestGameServer : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
    public Guid GameId { get; set; }
    public IEnumerable<Player> Players { get; set; }
    public string LspmKey { get; set; }
}
