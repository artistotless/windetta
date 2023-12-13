using Windetta.Common.Constants;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Exceptions;

public class WalletException : WindettaException
{
    public new string? Message { get; set; }

    public static WalletException FundsNotEnough
        => new WalletException(Errors.Wallet.FundsNotEnough);

    public static WalletException NotFound
        => new WalletException(Errors.Wallet.NotFound);

    public static WalletException InvalidFreezeAmount
        => new WalletException(Errors.Wallet.InvalidFreezeAmount);

    public static WalletException BalanceNotFound
        => new WalletException(Errors.Wallet.BalanceNotFound);

    public static WalletException FundsAlreadyHeld
        => new WalletException(Errors.Wallet.FundsAlreadyHeld);

    private WalletException(string errorCode) : base(errorCode) { }
}
