using LSPM.Core.Interfaces;
using LSPM.Core.Models;

namespace LSPM.Infrastructure.Services;

public class TestModeServerLauncher : IGameServerLauncher
{
    public ValueTask<GameServerInfo> LaunchAsync(Guid gameId)
    {
        return ValueTask.FromResult(new GameServerInfo()
        {
            Endpoint = new Uri("https://localhost:20001"),
            IpcEndpoint = new Uri("http://localhost:20002"),
            InstanceId = Guid.Parse("54575fee-c3bb-4e00-bc08-b35b6be53a7d"),
        });
    }
}
