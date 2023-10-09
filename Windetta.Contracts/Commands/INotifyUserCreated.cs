namespace Windetta.Contracts.Commands;

public interface INotifyUserCreated
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public DateTime TimeStamp { get; init; }
}
