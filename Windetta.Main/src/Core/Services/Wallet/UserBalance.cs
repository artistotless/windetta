namespace Windetta.Main.Core.Services.Wallet;

public class UserBalance
{
    public ulong Amount { get; init; }
    public ulong HeldAmount { get; init; }
    public ulong AvailableAmount => Math.Max(0, Amount - HeldAmount);

    public UserBalance(ulong amount, ulong heldAmount)
    {
        Amount = amount;
        HeldAmount = heldAmount;
    }
}