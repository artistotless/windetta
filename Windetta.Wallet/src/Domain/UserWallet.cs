using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Wallet.Domain;

public class UserWallet
{
    public Guid UserId { get; set; }
    public List<UserBalance>? Balances { get; set; }

    public void TransferToWallet(UserWallet to, int currencyId, long amount)
    {
        var balanceFrom = GetBalance(currencyId);
        var balanceTo = to.GetBalance(currencyId);

        balanceFrom?.Decrease(amount);
        balanceTo?.Increase(amount);
    }

    public UserBalance GetBalance(int currencyId)
    {
        var balance = Balances?
            .FirstOrDefault(x => currencyId == x.CurrencyId);

        if (balance is null)
        {
            throw new WindettaException(
        Errors.Wallet.BalanceNotFound,
        nameof(Errors.Wallet.BalanceNotFound));
        }

        return balance;
    }
}
