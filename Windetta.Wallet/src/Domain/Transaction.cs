namespace Windetta.Wallet.Domain;

public enum TransactionType
{
    TopUp,
    Withdrawal,
    CancelWithdrawal,
    Transfer
}

public class Transaction
{
    public Guid UserId { get; set; }
    public Guid Id { get; init; }
    public DateTime TimeStamp { get; set; }
    public TransactionType Type { get; set; }
    public int CurrencyId { get; set; }
    public long Amount { get; set; }
}
