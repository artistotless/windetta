namespace Windetta.Contracts.Commands;

public interface INotifyEmailConfirmation
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
}