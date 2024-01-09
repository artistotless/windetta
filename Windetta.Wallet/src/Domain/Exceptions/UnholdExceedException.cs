using Windetta.Common.Constants;

namespace Windetta.Wallet.Domain.Exceptions;

public class UnholdExceedException : DomainException
{
    public UnholdExceedException(string? message = null) :
        base(Errors.Wallet.UnholdProblem, message ?? 
        "The amount for unholding exceeds the actual held funds")
    {
    }
}

