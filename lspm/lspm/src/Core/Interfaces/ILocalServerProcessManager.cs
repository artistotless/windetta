using LSPM.Core.Models;

namespace LSPM.Core.Interfaces
{
    public interface ILocalServerProcessManager
    {
        public Task<GameServerInfo> GetOrLaunchGameServer(Guid gameId);
    }
}