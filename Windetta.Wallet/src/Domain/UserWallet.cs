using Windetta.Common.Types;

namespace Windetta.Wallet.Domain;

public class UserWallet
{
    public Guid UserId { get; init; }
    public long HeldBalance { get; private set; } = 0; // nanoton
    public TonAddress Address { get; init; }
    public WalletKeysSet WalletKeys { get; init; }

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
