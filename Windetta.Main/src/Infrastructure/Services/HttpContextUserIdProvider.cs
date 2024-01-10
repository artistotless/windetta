using Windetta.Main.Core.Services;

namespace Windetta.Main.Infrastructure.Services;

public class HttpContextUserIdProvider : IUserIdProvider
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

            if (!Guid.TryParse(_contextAccessor.HttpContext.User.Identity?.Name ?? string.Empty, out Guid id))
                throw new Exception
                    ("Cannot parse Guid userId from HttpContext");

            if (id == default)
                throw new Exception
                    ("userId from HttpContext is default guid value");

            return id;
        }
    }

}