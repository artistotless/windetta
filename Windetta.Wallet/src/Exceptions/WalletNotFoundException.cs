using Windetta.Common.Constants;

namespace Windetta.Wallet.Exceptions;

public class WalletNotFoundException : WalletException
{
    public WalletNotFoundException() : base(Errors.Wallet.NotFound,"Wallet not found")
    {
    }
}


