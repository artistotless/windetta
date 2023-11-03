using MassTransit;
using Windetta.Common.Types;

namespace Windetta.Contracts.Commands;

public interface INotifyTonWithdrawalExpired : CorrelatedBy<Guid>
{
    public Guid UserId { get; set; }
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
}
