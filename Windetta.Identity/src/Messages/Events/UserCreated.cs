using Windetta.Common.Messages;

namespace Windetta.Identity.Messages.Events;

public class UserCreated : IEvent
{
    public int MyProperty { get; set; }
}
