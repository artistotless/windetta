using Windetta.Main.Core.Games;

namespace Windetta.Main.Infrastructure.GameUIs;

public class InMemoryGameUIs : IGameUIs
{
    public Task AddAsync(Guid gameId, string uiContent)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetAsync(Guid gameId)
    {
        return GameUIContentLoader.GetUIContent(gameId).AsTask();
    }

    public Task RemoveAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid gameId, string uiContent)
    {
        throw new NotImplementedException();
    }
}
