using Windetta.Common.Types;

namespace Windetta.Wallet.Exceptions;

public class WalletException : WindettaException
{
    public WalletException(string errorCode) : base(errorCode)
    {
    }

    public WalletException(string errorCode, string message) : base(errorCode, message)
    {
    }
}


