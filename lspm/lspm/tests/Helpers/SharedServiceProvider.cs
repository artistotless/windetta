using LSPM.Core.Interfaces;
using LSPM.Core.Services;
using System;

namespace LspmTests.Helpers;

public static class SharedServiceProvider
{
    public static ServiceProvider GetProvider(Action<ServiceCollection>? configure = null)
    {
        var services = GetCollection(configure);

        return services.BuildServiceProvider();
    }

    public static ServiceCollection GetCollection(Action<ServiceCollection>? configure = null)
    {
        var services = new ServiceCollection();

        configure?.Invoke(services);
        services.TryAddSingleton<IGameServersOptions, InMemoryGameServersOptions>();
        services.TryAddSingleton<IGameServersStore, InMemoryGameServersStore>();
        services.TryAddSingleton(p => new ServerLauncherMock().Mock.Object);
        services.TryAddSingleton(p => new ServerNewMatchNotifierMock().Mock.Object);
        services.TryAddScoped<GameServersFacadeService>();
        services.TryAddScoped<ILocalServerProcessManager, LocalServerProcessManager>();

        return services;
    }
}