using Microsoft.AspNetCore.Identity;
using Windetta.Identity.Infrastructure.Exceptions;

namespace Windetta.Identity.Extensions;

public static class IdentityExtensions
{
    public static IdentityException FirstErrorAsException(this IEnumerable<IdentityError> errors)
    {
        var firstError = errors.First();

        return new IdentityException(firstError.Code, firstError.Description);
    }
}
