using System.Security.Authentication;
using System.Security.Claims;
using Windetta.Main.Core.Services;

namespace Windetta.Main.Infrastructure.Services;

public class HttpContextUserIdProvider : IUserIdService
{
    private readonly IHttpContextAccessor _contextAccessor;

    public HttpContextUserIdProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid UserId
    {
        get
        {
            if (_contextAccessor.HttpContext == null)
                throw new Exception
                    ("Cannot get userId from empty HttpContext");

            if (_contextAccessor.HttpContext.User.Identity == null ||
                !_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
                throw new AuthenticationException();

            var idClaim = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

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