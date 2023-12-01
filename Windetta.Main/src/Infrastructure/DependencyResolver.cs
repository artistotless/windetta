using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Infrastructure;

public static class DependencyResolver
{
    public static void AddDefaultInstanceIdProvider(this IServiceCollection services)
    {
        services.AddSingleton<IInstanceIdProvider, DefaultInstanceIdProvider>((p) =>
        {
            var cfg = p.GetRequiredService<IConfiguration>();
            var id = cfg.GetValue<string>("InstanceId");

            return new DefaultInstanceIdProvider(id);
        });
    }
}
