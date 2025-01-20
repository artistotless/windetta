using LSPM.Core.Exceptions;
using LSPM.Core.Interfaces;
using LSPM.Core.Models;

namespace LSPM.Infrastructure.Decorators;

public class LoadValidationDecorator : ILocalServerProcessManager
{
    private readonly ILocalServerProcessManager _wrappedObject;

    private readonly IGameServersOptions _optionsStore;
    private readonly IGameServersStore _gameServers;

    public LoadValidationDecorator(
        ILocalServerProcessManager wrapedObject,
        IGameServersOptions optionsStore,
        IGameServersStore gameServers)
    {
        _wrappedObject = wrapedObject;
        _optionsStore = optionsStore;
        _gameServers = gameServers;
    }

    public Task<GameServerInfo> GetOrLaunchGameServer(Guid gameId)
    {
        bool anyFreeLoad = _gameServers.AnyLoadFreeServer(gameId);

        if (!anyFreeLoad)
        {
            var instancesCount = _gameServers.Count(gameId);
            var options = _optionsStore.Get(gameId);
            if (instancesCount >= options.MaxInstances)
                throw LspmException.Overload;
        }

        return _wrappedObject.GetOrLaunchGameServer(gameId);
    }
}
