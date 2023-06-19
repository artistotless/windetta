using Windetta.Common.Types;

namespace Windetta.Identity.Infrastructure.Exceptions;

public class IdentityException : WindettaException
{
    public IdentityException(string errorCode, string message) : base(errorCode, message) { }
}
