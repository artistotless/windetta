namespace Windetta.Contracts;

public class BalanceOperationData : IHasOperationId
{
    public Guid UserId { get; set; }
    public FundsInfo Funds { get; set; }
    public Guid OperationId { get; init; }
}