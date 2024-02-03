namespace Windetta.Main.Infrastructure.Security;

internal static class AuthorizationConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        _ = services.AddAuthorization(options =>
        {
            Policies.Configure(options);
        });
    }
}
