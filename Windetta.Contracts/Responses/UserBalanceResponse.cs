namespace Windetta.Contracts.Responses;

public class UserBalanceResponse : ServiceResponse
{
    public ulong Amount { get; init; }
    public ulong HeldAmount { get; init; }
    public ulong AvailableAmount => Math.Max(0, Amount - HeldAmount);

    public UserBalanceResponse(ulong amount, ulong heldAmount)
    {
        Amount = amount;
        HeldAmount = heldAmount;
    }
}
