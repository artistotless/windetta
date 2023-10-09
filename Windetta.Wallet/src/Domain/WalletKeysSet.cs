namespace Windetta.Wallet.Domain;

public class WalletKeysSet
{
    public Guid UserId { get; init; }
    public UserWallet Wallet { get; init; }
    public string PrivateKey { get; init; }
    public string PublicKey { get; init; }
}