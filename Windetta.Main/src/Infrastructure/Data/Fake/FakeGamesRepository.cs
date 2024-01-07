using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;

namespace Windetta.Main.Infrastructure.Data.Fake;

public class FakeGamesRepository : IGames
{
    private readonly List<Game> _games;

    public static Guid ExampleId = Guid.Parse("b15739de-8d5b-4759-a20c-75389b14ecb8");

    public FakeGamesRepository()
    {
        _games = new List<Game>
        {
            new Game()
            {
                Id = ExampleId,
                Description = "Sea battle",
                IconPath = "/seabattle.png",
                Configuration = new GameConfiguration(2,1),
                Title = "Sea battle",
                SupportedCurrencies = new[] {
                    new SupportedCurrency() {
                        CurrencyId = 1, MaxBet = 1000, MinBet = 100
                    }}
            }
        };
    }

    public Task AddAsync(Game game)
    {
        _games.Add(game);

        return Task.CompletedTask;
    }

    public Task<Game?> GetAsync(Guid id)
    {
        var game = _games.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(game);
    }

    public Task RemoveAsync(Game game)
    {
        _games.Remove(game);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Game game)
    {
        return Task.CompletedTask;
    }

    public Task<(GameConfiguration, IEnumerable<SupportedCurrency>)> GetConfigurationsAsync(Guid id)
    {
        var configurations = _games
            .Where(g => g.Id == id)
            .Select(g => (g.Configuration, g.SupportedCurrencies))
            .FirstOrDefault();

        if (configurations.SupportedCurrencies is null)
            throw GameConfigurationException.NotFound;

        return Task.FromResult(configurations);
    }
}
