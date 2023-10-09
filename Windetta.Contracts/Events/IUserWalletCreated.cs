namespace Windetta.Contracts.Events;

public interface IUserWalletCreated
{
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
}