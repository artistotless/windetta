using System.Security.Authentication;
using System.Security.Claims;
using Windetta.Common.Types;
using Windetta.Main.Core.Services;

namespace Windetta.Main.Infrastructure.Services;

public class HttpContextUserIdProvider : IUserIdService
{
    protected readonly IHttpContextAccessor ContextAccessor;

    public HttpContextUserIdProvider(IHttpContextAccessor contextAccessor)
    {
        ContextAccessor = contextAccessor;
    }

    public virtual Guid UserId
    {
        get
        {
            if (ContextAccessor.HttpContext == null)
                throw new Exception
                    ("Cannot get userId from empty HttpContext");

            if (ContextAccessor.HttpContext.User.Identity == null ||
                !ContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                throw new AuthenticationException();

            var idClaim = ContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim == null)
                throw new Exception("Cannot get id claim from HttpContext");

            if (!Guid.TryParse(idClaim.Value, out Guid id))
                throw new Exception
                    ("Cannot parse Guid userId from HttpContext");

            if (id == default)
                throw new Exception
                    ("userId from HttpContext is default guid value");

            return id;
        }
    }

}