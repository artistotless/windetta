using Windetta.Wallet.Exceptions;

namespace Windetta.Wallet.Domain.Exceptions;

public class DomainException : WalletException
{
    public DomainException(string errorCode) : base(errorCode)
    {
    }

    public DomainException(string errorCode, string message) : base(errorCode, message)
    {
    }
}

