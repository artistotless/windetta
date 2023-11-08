using Windetta.Common.Messages;

namespace Windetta.Contracts.Commands;

public interface INotifyEmailConfirmation : ICommand
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
}