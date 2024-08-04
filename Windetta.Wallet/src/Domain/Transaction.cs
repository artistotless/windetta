namespace Windetta.Wallet.Domain;

public sealed class Transaction
{
    public Guid UserId { get; set; }
    public Guid Id { get; init; }
    public DateTime TimeStamp { get; set; }
    public TransactionType Type { get; set; }
    public int CurrencyId { get; set; }
    public ulong Amount { get; set; }
}