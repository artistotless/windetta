namespace Windetta.Wallet.Domain;

public class WalletCredential
{
    public Guid UserId { get; set; }
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
}