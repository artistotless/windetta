using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Wallet.Domain;

public class UserWallet
{
    public Guid UserId { get; init; }
    public long Balance { get; private set; } // nanotons
    public long HeldBalance { get; private set; } = 0; // nanotons

    #region Business logic
    public void IncreaseBalance(long amount)
    {
        Balance += amount;
    }

    public void HoldBalance(long amount)
    {
        if (HeldBalance != 0)
            throw new WindettaException(
                Errors.Wallet.FundsAlreadyHeld, "Funds already held");

        HeldBalance = amount;
    }

    public void TransferToWallet(UserWallet wallet, long amount)
    {
        this.DecreaseBalance(amount);
        wallet.IncreaseBalance(amount);
    }

    public void UnHoldBalance()
    {
        HeldBalance = 0;
    }

    public void DecreaseBalance(long amount)
    {
        if (amount <= 0)
            return;

        if (Balance - HeldBalance < amount)
            throw new WindettaException(
                Errors.Wallet.FundsNotEnough, "Insufficient funds");

        Balance -= amount;
    }
    #endregion
}
