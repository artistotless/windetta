﻿using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Wallet.Domain;

public class UserWallet
{
    public Guid UserId { get; init; }
    public List<UserBalance> Balances { get; set; }

    public void TransferToWallet(UserWallet to, FundsInfo funds)
    {
        var balanceFrom = GetBalance(funds.CurrencyId);
        var balanceTo = to.GetBalance(funds.CurrencyId);

        balanceFrom?.Decrease(funds.Amount);
        balanceTo?.Increase(funds.Amount);
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
