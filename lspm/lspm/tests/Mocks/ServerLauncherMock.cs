using LSPM.Core.Interfaces;
using LSPM.Core.Models;
using Windetta.Common.Testing;

namespace LspmTests.Mocks;

public class ServerLauncherMock : MockInitializator<IGameServerLauncher>
{
    protected override void Setup(Mock<IGameServerLauncher> mock)
    {
        var result = new GameServerInfo()
        {
            Endpoint = new Uri("https://localhost:9999"),
            InstanceId = Guid.NewGuid(),
            IpcEndpoint = new Uri("https://localhost:9090"),
        };

        mock.Setup(x => x.LaunchAsync(It.IsAny<Guid>()))
            .ReturnsAsync(result);
    }
}
