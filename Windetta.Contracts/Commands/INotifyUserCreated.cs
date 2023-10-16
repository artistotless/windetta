namespace Windetta.Contracts.Commands;

public interface INotifyUserCreated
{
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public DateTime TimeStamp { get; set; }
}
