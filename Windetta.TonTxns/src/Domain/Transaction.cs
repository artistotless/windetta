namespace Windetta.TonTxns.Domain;

public class Transaction
{
    public Guid Id { get; set; }
    public int TransfersCount { get; set; }
    public long TotalAmount { get; set; }
    public TransactionStatus Status { get; set; }
}
