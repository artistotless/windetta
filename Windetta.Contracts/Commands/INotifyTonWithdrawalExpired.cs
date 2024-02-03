using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Commands;

public interface INotifyTonWithdrawalExpired : CorrelatedBy<Guid>, ICommand
{
    public Guid UserId { get; set; }
    public ulong Nanotons { get; set; }
    public TonAddress Destination { get; set; }
}
