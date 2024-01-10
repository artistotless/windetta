using Polly;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Lobbies.UseCases;
using Windetta.Main.Core.Services.LSPM;
using Windetta.Main.Infrastructure.Consumers;
using Windetta.Main.Infrastructure.Lobby.Plugins;
using Windetta.Main.Infrastructure.Retries;
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

    public static void AddLobby(this IServiceCollection services)
    {
        services.AddScoped<ILobbyUseCase, Create>();
        services.AddScoped<ILobbyUseCase, Delete>();
        services.AddScoped<ILobbyUseCase, Get>();
        services.AddScoped<ILobbyUseCase, GetAll>();
        services.AddScoped<ILobbyUseCase, GetLobbyIdByUserId>();
        services.AddScoped<IGetLobbyIdByUserIdUseCase, GetLobbyIdByUserId>();
        services.AddScoped<ILobbyUseCase, JoinMember>();
        services.AddScoped<ILobbyUseCase, LeaveMember>();
    }

    public static void AddLobbyPlugins(this IServiceCollection services)
    {
        services.AddScoped<ILobbyPlugin, DefaultDisposeStrategy>();
        services.AddScoped<ILobbyPlugin, DefaultReadyStrategy>();
        services.AddScoped<ILobbyPlugin, RoleJoinFilter>();
        services.AddScoped<ILobbyPlugin, DateReadyStrategy>();
    }

    public static void AddInMemoryLspms(this IServiceCollection services)
    {
        services.AddScoped<ILspms, InMemoryLspms>();
    }

    public static void AddPolyRetries(this IServiceCollection services)
    {
        services.AddResiliencePipeline(typeof(SearchGameServerConsumer),
            PollyPipelines.AddSearchGameServerConsumerRetryPolicy);
    }
}
