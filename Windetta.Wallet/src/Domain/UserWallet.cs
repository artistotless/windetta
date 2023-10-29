namespace Windetta.Wallet.Domain;

public class UserWallet
{
    public Guid UserId { get; init; }
    public long Balance { get; private set; }
    public long HeldBalance { get; private set; } = 0; // nanoton

    public void IncreaseBalance(long nanotons)
    {
        Balance += nanotons;
    }

    public void HoldBalance(long nanotons)
    {
        if (HeldBalance != 0)
            throw new InvalidOperationException("Balance already held");

        HeldBalance = nanotons;
    }

    public void UnHoldBalance()
    {
        HeldBalance = 0;
    }
}
