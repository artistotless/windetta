using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IUserEmailConfirmed : IEvent
{
    public Guid UserId { get; set; }
}
