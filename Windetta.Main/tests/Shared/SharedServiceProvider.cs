using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests.Shared;

public static class SharedServiceProvider
{
    public static IServiceProvider GetInstance(Action<ServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();

        configure?.Invoke(services);

        var provider = services.BuildServiceProvider();

        // dependencies
        services.TryAddScoped(p => new WalletServiceMock().Mock.Object);
        services.TryAddSingleton<IGames, InMemoryGamesRepository>();
        services.TryAddSingleton<IMatchHubs, InMemoryMatchHubsStorage>();
        services.TryAddSingleton<IMatchHubUsersAssociations, InMemoryMatchHubUsersAssociations>();

        // plugins
        services.AddIfNotAddedScoped<IMatchHubPlugin, MinUserBalanceJoinFilter>(provider);
        services.AddIfNotAddedScoped<IMatchHubPlugin, AlwaysFalseJoinFilter>(provider);
        services.AddIfNotAddedScoped<IMatchHubPlugin, FullRoomsReadyStrategy>(provider);
        services.AddIfNotAddedScoped<IMatchHubPlugin, DateDisposeStrategy>(provider);

        // use cases
        services.AddIfNotAddedScoped<IMatchHubUseCase, GetHubIdByUserId>(provider);
        services.AddIfNotAddedScoped<IMatchHubUseCase, Create>(provider);
        services.AddIfNotAddedScoped<IMatchHubUseCase, Delete>(provider);
        services.AddIfNotAddedScoped<IMatchHubUseCase, Get>(provider);
        services.AddIfNotAddedScoped<IMatchHubUseCase, GetAll>(provider);
        services.AddIfNotAddedScoped<IMatchHubUseCase, LeaveMember>(provider);
        services.AddIfNotAddedScoped<IMatchHubUseCase, JoinMember>(provider);

        services.TryAddScoped<IMatchHubPluginsFactory, DefaultMatchHubPluginsFactory>();
        services.TryAddScoped<IMatchHubUseCasesFactory, DefaultMatchHubsUseCaseFactory>();

        services.TryAddScoped<MatchHubsInteractor>();

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

