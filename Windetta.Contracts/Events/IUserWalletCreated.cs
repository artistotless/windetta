using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IUserWalletCreated : IEvent
{
    public Guid UserId { get; set; }
    public DateTime TimeStamp { get; set; }
}