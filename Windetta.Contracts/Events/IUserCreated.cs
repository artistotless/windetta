using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IUserCreated : IEvent, CorrelatedBy<Guid>
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime TimeStamp { get; set; }
}
