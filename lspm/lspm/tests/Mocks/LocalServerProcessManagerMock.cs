using LSPM.Core.Interfaces;
using LSPM.Core.Models;
using Windetta.Common.Testing;

namespace LspmTests.Mocks;

public class LocalServerProcessManagerMock : MockInitializator<ILocalServerProcessManager>
{
    public Uri? ReturnEndpoint { get; set; }

    protected override void Setup(Mock<ILocalServerProcessManager> mock)
    {
        mock.Setup(x => x.GetOrLaunchGameServer(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new GameServerInfo()
            {
                InstanceId = Guid.NewGuid(),
                IpcEndpoint = new Uri("udp://127.0.0.1:9090"),
                Endpoint = ReturnEndpoint ?? new Uri("udp://127.0.0.1:9090"),
            });
    }
}