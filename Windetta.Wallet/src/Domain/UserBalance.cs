using Windetta.Wallet.Domain.Exceptions;

namespace Windetta.Wallet.Domain;

public class UserBalance
{
    public Guid WalletId { get; init; }
    public int CurrencyId { get; init; }
    public ulong Amount { get; private set; } = 0;
    public ulong HeldAmount { get; private set; } = 0;
    private ulong _availableAmount => Amount - HeldAmount;

    public void Increase(ulong value)
    {
        Amount += value;
    }

    public void Hold(ulong amount)
    {
        if (amount > _availableAmount)
            throw new FundsNotEnoughException();

        HeldAmount += amount;
    }

    public void UnHold(ulong amount)
    {
        if (amount > HeldAmount)
            throw new UnholdExceedException();

        HeldAmount -= amount;
    }

    public void Decrease(ulong value)
    {
        if (value <= 0)
            return;

        if (value > _availableAmount)
            throw new FundsNotEnoughException();

        Amount -= value;
    }
}
