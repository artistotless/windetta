using Windetta.Common.Constants;

namespace Windetta.Wallet.Domain.Exceptions;

public class FundsNotEnoughException : DomainException
{
    public FundsNotEnoughException(string? message = null) :
        base(Errors.Wallet.FundsNotEnough, message ?? "Insufficient funds")
    {
    }
}