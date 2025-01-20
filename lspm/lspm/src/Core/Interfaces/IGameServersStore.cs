using LSPM.Core.Models;

namespace LSPM.Core.Interfaces;

public interface IGameServersStore
{
    void Add(GameServerEntry server);
    IEnumerable<GameServerEntry> GetAll();
    GameServerEntry? Find(Guid instanceId);
    GameServerEntry? GetLoadFreeServer(Guid gameId);
    bool AnyLoadFreeServer(Guid gameId);
    void Remove(Guid instanceId);
    int Count(Guid gameId);
}
