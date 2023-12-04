namespace Windetta.Main.Infrastructure.Security;

internal static class AuthorizationExtensions
{
    public static void ConfigureAddAuthorization(this IServiceCollection services)
    {
        _ = services.AddAuthorization(options =>
        {
            Policies.Configure(options);
        });
    }
}
