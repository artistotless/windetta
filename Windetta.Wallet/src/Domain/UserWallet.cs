using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Wallet.Domain;

public class UserWallet
{
    public Guid UserId { get; init; }
    public long Balance { get; private set; } // nanotons
    public long HeldBalance { get; private set; } = 0; // nanotons

    #region Business logic
    public void IncreaseBalance(long nanotons)
    {
        Balance += nanotons;
    }

    public void HoldBalance(long nanotons)
    {
        if (HeldBalance != 0)
            throw new WindettaException(
                Errors.Wallet.FundsAlreadyHeld, "Funds already held");

        HeldBalance = nanotons;
    }

    public void TransferToWallet(UserWallet wallet, long nanotons)
    {
        this.DecreaseBalance(nanotons);
        wallet.IncreaseBalance(nanotons);
    }

    public void UnHoldBalance()
    {
        HeldBalance = 0;
    }

    private void DecreaseBalance(long nanotons)
    {
        if (nanotons <= 0)
            return;

        if (Balance - HeldBalance < nanotons)
            throw new WindettaException(
                Errors.Wallet.FundsNotEnough, "Insufficient funds");

        Balance -= nanotons;
    }
    #endregion
}
