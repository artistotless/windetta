using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Wallet.Domain;

public class UserBalance
{
    public Guid WalletId { get; init; }
    public int CurrencyId { get; init; }
    public long Amount { get; private set; } = 0;
    public long HeldAmount { get; private set; } = 0;

    #region Business logic
    public void Increase(long value)
    {
        Amount += value;
    }

    public void Hold(long amount)
    {
        if (HeldAmount != 0)
            throw new WindettaException(
                Errors.Wallet.FundsAlreadyHeld, "Funds already held");

        HeldAmount = amount;
    }

    public void UnHold()
    {
        HeldAmount = 0;
    }

    public void Decrease(long value)
    {
        if (value <= 0)
            return;

        if (Amount - HeldAmount < value)
            throw new WindettaException(
                Errors.Wallet.FundsNotEnough, "Insufficient funds");

        Amount -= value;
    }
    #endregion
}
