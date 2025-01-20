using LSPM.Core.Models;

namespace LSPM.Core.Interfaces;

public interface IGameServersOptions
{
    GameServerOptions Get(Guid gameId);
}
