using Polly;
using Windetta.Main.Core.MatchHubs.Plugins;
using Windetta.Main.Core.MatchHubs.UseCases;
using Windetta.Main.Core.Services.LSPM;
using Windetta.Main.Infrastructure.MatchHub.Plugins;
using Windetta.Main.Infrastructure.Retries;
using Windetta.Main.Infrastructure.Sagas.Activities;
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

    public static void AddMatchHub(this IServiceCollection services)
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
        services.AddScoped<IMatchHubPlugin, DefaultDisposeStrategy>();
        services.AddScoped<IMatchHubPlugin, DefaultReadyStrategy>();
        services.AddScoped<IMatchHubPlugin, RoleJoinFilter>();
        services.AddScoped<IMatchHubPlugin, DateReadyStrategy>();
    }

    public static void AddInMemoryLspms(this IServiceCollection services)
    {
        services.AddScoped<ILspms, InMemoryLspms>();
    }

    public static void AddPolyRetries(this IServiceCollection services)
    {
        services.AddResiliencePipeline(typeof(SearchingGameServerActivity),
            PollyPipelines.SearchingGameServerActivity);
    }
}
