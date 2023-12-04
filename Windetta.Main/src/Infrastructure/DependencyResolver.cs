using Windetta.Main.Core.MatchHubs;
using Windetta.Main.Core.MatchHubs.UseCases;
using Windetta.Main.Infrastructure.MatchHubPlugins;
using Windetta.Main.Infrastructure.Services;
using Windetta.Main.MatchHubs.Strategies;

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

    public static void AddMatchHubUseCases(this IServiceCollection services)
    {
        services.AddScoped<IMatchHubUseCase, Create>();
        services.AddScoped<IMatchHubUseCase, Delete>();
        services.AddScoped<IMatchHubUseCase, Get>();
        services.AddScoped<IMatchHubUseCase, GetAll>();
        services.AddScoped<IMatchHubUseCase, GetHubIdByUserId>();
        services.AddScoped<IGetMatchHubIdByUserIdUseCase, GetHubIdByUserId>();
        services.AddScoped<IMatchHubUseCase, JoinMember>();
        services.AddScoped<IMatchHubUseCase, LeaveMember>();
    }

    public static void AddMatchHubPlugins(this IServiceCollection services)
    {
        services.AddScoped<IMatchHubPlugin, DefaultReadyStrategy>();
        services.AddScoped<IMatchHubPlugin, RoleJoinFilter>();
        services.AddScoped<IMatchHubPlugin, DateReadyStrategy>();
    }
}
