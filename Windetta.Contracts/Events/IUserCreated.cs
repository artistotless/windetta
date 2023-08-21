using MassTransit;

namespace Windetta.Contracts.Events;

public interface IUserCreated
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
