using Windetta.Common.Testing;
using Windetta.Main.Core.Services.LSPM;

namespace Windetta.MainTests.Mocks;

public class LspmsMock : MockInitializator<ILspms>
{
    private readonly InMemoryLspms _storage;

    public LspmsMock(List<Lspm>? initial = null)
    {
        _storage = new InMemoryLspms(initial);
    }

    protected override void Setup(Mock<ILspms> mock)
    {
        mock
            .Setup(x => x.GetAllAsync())
            .Returns(async () => await _storage.GetAllAsync());

        mock
            .Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .Returns(async (Guid gameId) => await _storage.GetAsync(gameId));

        mock
            .Setup(x => x.GetAllAsync(It.IsAny<Guid>()))
            .Returns(async (Guid gameId) => await _storage.GetAllAsync(gameId));
    }
}
