using Microsoft.AspNetCore.Identity;
using Windetta.Common.Types;

namespace Windetta.Identity.Extensions;

public static class IdentityExtensions
{
    public static WindettaException FirstErrorAsException(this IEnumerable<IdentityError> errors)
    {
        var firstError = errors.First();

        return new WindettaException(firstError.Description, firstError.Code);
    }
}
