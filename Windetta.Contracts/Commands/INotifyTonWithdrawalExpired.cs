using MassTransit;
using Windetta.Common.Types;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface INotifyTonWithdrawalExpired : CorrelatedBy<Guid>, ICommand
{
    public Guid UserId { get; set; }
    public long Nanotons { get; set; }
    public TonAddress Destination { get; set; }
}
