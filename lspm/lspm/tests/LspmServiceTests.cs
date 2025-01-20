using LSPM.Core.Exceptions;
using LSPM.Core.Interfaces;
using LSPM.Infrastructure.Decorators;
using LSPM.Models;
using Windetta.Common.Constants;
using Windetta.Common.Testing;

namespace LspmTests;

public class LspmServiceTests
{
    [Fact]
    public async Task OverloadCase_ShouldThrowException()
    {
        // arrange
        var services = SharedServiceProvider.GetCollection(services =>
        {
            services.AddSingleton<IGameServersOptions>
            (p => InMemoryGameServersOptions.OverloadCase);
        });

        var autofac = new ContainerBuilder();
        autofac.RegisterDecorator<LoadValidationDecorator, ILocalServerProcessManager>();
        autofac.Populate(services);
        var provider = autofac.Build();

        var lspm = provider.Resolve<ILocalServerProcessManager>();

        // act
        var exception = await Should.ThrowAsync<LspmException>(
            () => lspm.GetOrLaunchGameServer(ExampleGuids.GameId));

        // assert
        exception.ErrorCode.ShouldBe(Errors.LSPM.Overload);
    }

    [Fact]
    public async Task ShouldHitLauncher_If_NoInstances()
    {
        // arrange
        var launcherMock = new ServerLauncherMock().Mock;
        using var provider = SharedServiceProvider.GetProvider(services =>
        {
            services.AddSingleton(p => launcherMock.Object);
        });
        var sut = provider.GetRequiredService<ILocalServerProcessManager>();
        var players = new[] { new Player() };
        var matchId = Guid.NewGuid();

        // act
        _ = await sut.GetOrLaunchGameServer(ExampleGuids.GameId);

        // assert
        launcherMock.Verify(x => x.LaunchAsync(It.IsAny<Guid>()));
    }
}
