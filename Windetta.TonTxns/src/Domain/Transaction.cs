using Windetta.Common.Types;

namespace Windetta.TonTxns.Domain;

public class Transaction : AuditableEntity
{
    public Guid Id { get; set; }
    public int TransfersCount { get; set; }
    public long TotalAmount { get; set; }
    public TransactionStatus Status { get; set; }
}
