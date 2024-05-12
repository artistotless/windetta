using Windetta.Main.Infrastructure.Security;

namespace Windetta.Main.Infrastructure.Config;

internal static class AuthorizationConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddAuthorization(options =>
       {
           Policies.Configure(options);
       });
    }
}
