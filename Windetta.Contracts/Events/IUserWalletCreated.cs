using Windetta.Common.Types;

namespace Windetta.Contracts.Events;

public interface IUserWalletCreated
{
    public Guid UserId { get; set; }
    public TonAddress Address { get; set; }
    public DateTime TimeStamp { get; set; }
}