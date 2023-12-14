using MassTransit;
using Windetta.Common.Messages;
using Windetta.Main.Core.Matches;

namespace Windetta.Main.Core.Services.LSPM;

public class StartSearchingGameServer : CorrelatedBy<Guid>, ICommand
{
    public Guid CorrelationId { get; set; }
    public Guid GameId { get; set; }
    public IEnumerable<Player> Players { get; set; }
}