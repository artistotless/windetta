using MassTransit;
using Windetta.Contracts.Events;

namespace Windetta.Identity.Messages.Events;

//[ConfigureConsumeTopology(false)]
public class UserCreated : IUserCreated
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}