namespace Windetta.Wallet.Domain;

public enum TransactionType
{
    TopUp,
    Withdraw,
    Transfer
}

public class Transaction
{
    public Guid UserId { get; set; }
    public string Id { get; init; }
    public DateTime TimeStamp { get; set; }
    public TransactionType Type { get; set; }
    public long Nanotons { get; set; }
}
