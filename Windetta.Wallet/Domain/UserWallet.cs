namespace Windetta.Wallet.Domain;

public class UserWallet
{
    public Guid UserId { get; set; }
    public long HeldBalance { get; set; } // nanoton
    public string Address { get; set; }
}