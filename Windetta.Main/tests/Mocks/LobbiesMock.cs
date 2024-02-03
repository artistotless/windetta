using Windetta.Common.Testing;
using Windetta.Main.Core.Lobbies;

namespace Windetta.MainTests.Mocks;

public class LobbiesMock : MockInitializator<ILobbies>
{
    public ILobby? ReturnThisLobby { get; set; }

    private readonly InMemoryLobbiesStorage _storage;
    public LobbiesMock()
    {
        _storage = new InMemoryLobbiesStorage();
    }

    protected override void Setup(Mock<ILobbies> mock)
    {
        mock.Setup(x => x.AddAsync(It.IsAny<ILobby>()))
            .Returns(async (ILobby h) => await _storage.AddAsync(h));

        mock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .Returns<Guid>(async id => ReturnThisLobby ?? await _storage.GetAsync(id));

        mock.Setup(x => x.RemoveAsync(It.IsAny<Guid>()))
            .Returns<Guid>(async id => await _storage.RemoveAsync(id));

        mock.Setup(x => x.UpdateAsync(It.IsAny<ILobby>()))
            .Returns<ILobby>(async h => await _storage.UpdateAsync(h));

        mock.Setup(x => x.GetAllAsync())
            .Returns(async () => await _storage.GetAllAsync());
    }
}
