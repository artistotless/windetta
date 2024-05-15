using Windetta.Main.Core.Games;
using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Infrastructure.GameUIs;

public class InMemoryGameUIs : IGameUIs
{
    public Task AddAsync(Guid gameId, GameUIResult uiContent)
    {
        throw new NotImplementedException();
    }

    public Task<GameUIResult> GetAsync(Guid gameId)
    {
        return GameUIContentLoader.GetUIContent(gameId).AsTask();
    }

    public Task RemoveAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid gameId, GameUIResult uiContent)
    {
        throw new NotImplementedException();
    }
}
