using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IUserEmailConfirmed : IEvent
{
    public Guid UserId { get; set; }
}
