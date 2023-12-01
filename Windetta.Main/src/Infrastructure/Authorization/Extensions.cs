namespace Windetta.Main.Infrastructure.Authorization;

internal static class Extensions
{
    public static void ConfigureAddAuthorization(this IServiceCollection services)
    {
        _ = services.AddAuthorization(options =>
        {
            Policies.Configure(options);
        });
    }
}
