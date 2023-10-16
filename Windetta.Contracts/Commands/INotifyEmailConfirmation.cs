namespace Windetta.Contracts.Commands;

public interface INotifyEmailConfirmation
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
}