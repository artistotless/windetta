using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.Services;

public sealed class FromHeaderUserIdProvider : HttpContextUserIdProvider, ISingletonService
{
    public FromHeaderUserIdProvider(IHttpContextAccessor contextAccessor) : base(contextAccessor)
    {
    }

    public override Guid UserId
    {
        get
        {
            if (ContextAccessor.HttpContext == null)
                throw new Exception
                    ("Cannot get userId from empty HttpContext");

            return FromContext(ContextAccessor.HttpContext);
        }
    }

    public Guid FromContext(HttpContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (!context.Request.Headers.TryGetValue("X-UserId", out var userIdHeader))
            throw new Exception
                ("Cannot get userId from header. Add 'X-UserId' header for request!");

        return Guid.Parse(userIdHeader.ToString());
    }
}
