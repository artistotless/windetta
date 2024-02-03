using Windetta.Common.Constants;

namespace Windetta.Wallet.Exceptions;

public class BalanceNotFoundException : WalletException
{
    public BalanceNotFoundException(string? message = null) :
        base(Errors.Wallet.BalanceNotFound, message ??
        "There is no balance in the wallet for the specified currency")
    {
    }
}

