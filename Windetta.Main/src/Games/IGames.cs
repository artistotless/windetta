namespace Windetta.Main.Games;

public interface IGames
{
    public Task Add(Game game);
    public Task Remove(Game game);
    public Task Update(Game game);
    public Task<Game> Get(Guid id);
}