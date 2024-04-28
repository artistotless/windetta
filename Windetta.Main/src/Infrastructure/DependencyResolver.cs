using Microsoft.AspNetCore.SignalR;
using Polly;
using System.Reflection;
using Windetta.Common.Database;
using Windetta.Common.Mongo;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Lobbies.UseCases;
using Windetta.Main.Core.Services.LSPM;
using Windetta.Main.Infrastructure.Consumers;
using Windetta.Main.Infrastructure.Lobby.Plugins;
using Windetta.Main.Infrastructure.Logging;
using Windetta.Main.Infrastructure.MassTransit;
using Windetta.Main.Infrastructure.Retries;
using Windetta.Main.Infrastructure.Sagas;
using Windetta.Main.Infrastructure.Security;
using Windetta.Main.Infrastructure.Services;
using Windetta.Main.Infrastructure.SignalR;
using IUserIdProvider = Windetta.Common.Authentication.IUserIdProvider;

namespace Windetta.Main.Infrastructure;

public static class DependencyResolver
{
    public static void AddInfrastructureLayer(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var assembly = Assembly.GetExecutingAssembly();

        builder.ConfigureAddLogging();
        services.AddUserIdProvider();
        services.AddDefaultInstanceIdProvider();
        services.AddConfigureAuthorization();
        services.AddConfigureAuthentication();
        services.ConfigureAddSignalR();
        services.AddHttpContextAccessor();
        services.AddConfigureCors();
        services.AddLobby();
        services.AddLobbyPlugins();
        services.AddMongo();
        services.AddPolyRetries();
        services.AddMysqlDbContext<SagasDbContext>(assembly);
        services.AddInMemoryLspms();
        services.AddConfigureMassTransit(assembly);
    }

    private static void AddConfigureCors(this IServiceCollection services)
        => CorsConfiguration.Configure(services);

    private static void AddConfigureAuthentication(this IServiceCollection services)
        => AuthenticationConfiguration.Configure(services);

    private static void AddConfigureAuthorization(this IServiceCollection services)
        => AuthorizationConfiguration.Configure(services);

    private static void AddConfigureMassTransit(this IServiceCollection services, Assembly assembly)
    => MassTransitConfiguration.Configure(services, assembly);

    private static void AddDefaultInstanceIdProvider(this IServiceCollection services)
    {
        services.AddSingleton<IInstanceIdProvider, DefaultInstanceIdProvider>((p) =>
        {
            var cfg = p.GetRequiredService<IConfiguration>();
            var id = cfg.GetValue<string>("InstanceId");

            return new DefaultInstanceIdProvider(id);
        });
    }

    private static void AddLobby(this IServiceCollection services)
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

    private static void AddLobbyPlugins(this IServiceCollection services)
    {
        services.AddScoped<ILobbyPlugin, DefaultDisposeStrategy>();
        services.AddScoped<ILobbyPlugin, DefaultReadyStrategy>();
        services.AddScoped<ILobbyPlugin, RoleJoinFilter>();
        services.AddScoped<ILobbyPlugin, DateReadyStrategy>();
    }

    private static void AddInMemoryLspms(this IServiceCollection services)
    {
        services.AddScoped<ILspms, InMemoryLspms>();
    }

    private static void AddPolyRetries(this IServiceCollection services)
    {
        services.AddResiliencePipeline(typeof(SearchGameServerConsumer),
            PollyPipelines.AddSearchGameServerConsumerRetryPolicy);
    }

    private static void AddUserIdProvider(this IServiceCollection services)
    {
        services.AddScoped<IUserIdProvider, FromHeaderUserIdProvider>();
    }

    private static void ConfigureAddLogging(this WebApplicationBuilder builder)
        => LoggingConfiguration.ConfigureAddLogging(builder);

    private static void ConfigureAddSignalR(this IServiceCollection services)
    {
        services.AddSignalR(hubOptions =>
        {
            hubOptions.AddFilter<SignalRExceptionFilter>();
        });

        services.AddSingleton<SignalRExceptionFilter>();
    }
}
