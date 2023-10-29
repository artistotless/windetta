namespace Windetta.Contracts.Events;

public interface IFundsAdded
{
    public string TxnId { get; set; }
    public Guid UserId { get; set; }
    public long Nanotons { get; set; }
}
