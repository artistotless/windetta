using MassTransit;
using Windetta.Common.Types;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IWithdrawTonRequested : CorrelatedBy<Guid>, IEvent
{
    public Guid UserId { get; set; }
    public TonAddress Destination { get; set; }
    public ulong Nanotons { get; set; }
}