using Windetta.Common.Constants;
using Windetta.Common.Messages;
using Windetta.Common.RabbitMQ;

namespace Windetta.Wallet.Messages.Events;

[MessageNamespace(Services.Identity)]
public class UserCreated : IEvent
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
