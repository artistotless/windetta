using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Windetta.Main.Core.Games;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Plugins;
using Windetta.Main.Core.Lobbies.UseCases;
using Windetta.Main.Core.Matches;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.Shared;

public static class SharedServiceProvider
{
    public static ServiceProvider GetInstance(Action<ServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();

        configure?.Invoke(services);

        var provider = services.BuildServiceProvider();

        // dependencies
        services.TryAddScoped(p => new WalletServiceMock().Mock.Object);
        services.TryAddSingleton<IOngoingMatches, InMemoryOngoingMatches>();
        services.TryAddSingleton<IGames, InMemoryGamesRepository>();
        services.TryAddSingleton<ILobbies, InMemoryLobbiesStorage>();
        services.TryAddSingleton<ILobbyUsersAssociations, InMemoryLobbyUsersAssociations>();

        // plugins
        services.AddIfNotAddedScoped<ILobbyPlugin, MinUserBalanceJoinFilter>(provider);
        services.AddIfNotAddedScoped<ILobbyPlugin, AlwaysFalseJoinFilter>(provider);
        services.AddIfNotAddedScoped<ILobbyPlugin, FullRoomsReadyStrategy>(provider);
        services.AddIfNotAddedScoped<ILobbyPlugin, DateDisposeStrategy>(provider);

        // use cases
        services.AddIfNotAddedScoped<ILobbyUseCase, GetLobbyIdByUserId>(provider);
        services.AddIfNotAddedScoped<ILobbyUseCase, Create>(provider);
        services.AddIfNotAddedScoped<ILobbyUseCase, Delete>(provider);
        services.AddIfNotAddedScoped<ILobbyUseCase, Get>(provider);
        services.AddIfNotAddedScoped<ILobbyUseCase, GetAll>(provider);
        services.AddIfNotAddedScoped<ILobbyUseCase, LeaveMember>(provider);
        services.AddIfNotAddedScoped<ILobbyUseCase, JoinMember>(provider);

        services.TryAddScoped<ILobbyPluginsFactory, DefaultLobbyPluginsFactory>();
        services.TryAddScoped<ILobbyUseCasesFactory, DefaultLobbiesUseCaseFactory>();

        services.TryAddScoped<LobbiesInteractor>();

        return services.BuildServiceProvider();
    }

    private static void AddIfNotAddedScoped<TService, TImplementation>(
        this IServiceCollection services, ServiceProvider provider)
            where TService : class
            where TImplementation : class, TService
    {
        var foundServices = provider.GetServices<TService>();

        if (!foundServices.Any(x => x is TImplementation))
        {
            services.AddScoped<TService, TImplementation>();

            return;
        }
    }
}
