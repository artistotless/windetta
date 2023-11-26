using Windetta.Main.MatchHub;
using Windetta.MainTests.Mocks;

namespace Windetta.MainTests;

public class MatchHubInteractorTests
{
    [Fact]
    public async Task ShouldAddHubToStorageWhenCreate()
    {
        // arrange
        var storage = new MatchHubsInMemoryStorage();
        var interactor = new MatchHubsInteractor(storage);

        // act
        await interactor.CreateAsync(new MatchHubOptions());

        // assert
        storage.Count.ShouldBe(1);
    }

    [Fact]
    public async Task ShouldRemoveHubFromStorageWhenDelete()
    {
        // arrange
        var storage = new MatchHubsInMemoryStorage();
        var interactor = new MatchHubsInteractor(storage);

        // act
        var hub = await interactor.CreateAsync(new MatchHubOptions());
        await interactor.DeleteAsync(hub.Id);

        // assert
        storage.Count.ShouldBe(0);
    }
}
