using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Events;

public class UserCreated : IEvent
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
