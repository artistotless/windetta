using Windetta.Common.Testing;

namespace Windetta.MainTests.Mocks;

public class MatchHubsMock : MockInitializator<IMatchHubs>
{
    public IMatchHub? ReturnThisHub { get; set; }

    private readonly InMemoryMatchHubsStorage _storage;
    public MatchHubsMock()
    {
        _storage = new InMemoryMatchHubsStorage();
    }

    protected override void Setup(Mock<IMatchHubs> mock)
    {
        mock.Setup(x => x.AddAsync(It.IsAny<IMatchHub>()))
            .Returns(async (IMatchHub h) => await _storage.AddAsync(h));

        mock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .Returns<Guid>(async id => ReturnThisHub ?? await _storage.GetAsync(id));

        mock.Setup(x => x.RemoveAsync(It.IsAny<Guid>()))
            .Returns<Guid>(async id => await _storage.RemoveAsync(id));

        mock.Setup(x => x.UpdateAsync(It.IsAny<IMatchHub>()))
            .Returns<IMatchHub>(async h => await _storage.UpdateAsync(h));

        mock.Setup(x => x.GetAllAsync())
            .Returns(async () => await _storage.GetAllAsync());
    }
}
