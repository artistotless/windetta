using Windetta.Contracts;

namespace Windetta.Wallet.Domain;

public sealed class UserWallet
{
    public Guid UserId { get; init; }
    public DateTime Created { get; set; }
    public List<UserBalance> Balances { get; set; }

    public UserWallet()
    {
        Created = DateTime.UtcNow;
    }

    public void TransferToWallet(UserWallet to, FundsInfo funds)
    {
        var balanceFrom = GetBalance(funds.CurrencyId);
        var balanceTo = to.GetBalance(funds.CurrencyId);

        balanceFrom?.Decrease(funds.Amount);
        balanceTo?.Increase(funds.Amount);
    }

    public UserBalance? GetBalance(int currencyId)
    {
        var balance = Balances?
            .FirstOrDefault(x => currencyId == x.CurrencyId);

        return balance;
    }
}
