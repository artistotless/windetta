using MassTransit;
using Windetta.Common.Types;

namespace Windetta.Contracts.Events;

public interface IWithdrawTonRequested : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public TonAddress Destination { get; set; }
    public long Nanotons { get; set; }
}
