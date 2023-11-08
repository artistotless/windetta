using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface INotifyUserCreated : ICommand
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime TimeStamp { get; set; }
}
