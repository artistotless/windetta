using LSPM.Models;
using LSPM.Core.Interfaces;
using Windetta.Common.Testing;

namespace LspmTests.Mocks;

public class ServerNewMatchNotifierMock : MockInitializator<IGameServerMatchClient>
{
    protected override void Setup(Mock<IGameServerMatchClient> mock)
    {
        mock.Setup(x => x.CreateMatchAsync(It.IsAny<Uri>(), It.IsAny<MatchInitializationData>()))
            .Returns(Task.CompletedTask);
    }
}