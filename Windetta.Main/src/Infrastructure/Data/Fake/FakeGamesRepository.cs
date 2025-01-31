﻿using Windetta.Common.Testing;
using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.Games;

namespace Windetta.Main.Infrastructure.Data.Fake;

public class FakeGamesRepository : IGames
{
    private readonly List<Game> _games;

    public FakeGamesRepository()
    {
        _games = new List<Game>
        {
            new Game()
            {
                Id = ExampleGuids.GameId,
                Description = "Sea battle",
                IconPath = "/seabattle.png",
                Configuration = new GameConfiguration(1, 1){MinTeams=2, MaxTeams=2},
                Title = "Sea battle",
                SupportedCurrencies = new[] {new SupportedCurrency() {CurrencyId = 1, MaxBet = 1000, MinBet = 100 }}
            },

             new Game()
            {
                Id = Guid.Parse("e5586b7b-4cc0-490d-8b11-220274e7ce83"),
                Description = "TestGame",
                IconPath = "/test_game.png",
                Configuration = new GameConfiguration(1, 1){MinTeams=2, MaxTeams=2},
                Title = "TestGame",
                SupportedCurrencies = new[] {new SupportedCurrency() {CurrencyId = 1, MaxBet = 1000, MinBet = 100 }}
            },

             new Game()
            {
                Id = Guid.Parse("b9d9e5fd-0079-4f45-9347-d09b005ed498"),
                Description = "FoolGame",
                IconPath = "/foolgame.png",
                Configuration = new GameConfiguration(1, 1){MinTeams=2, MaxTeams=2},
                Title = "FoolGame",
                SupportedCurrencies = new[] {new SupportedCurrency() {CurrencyId = 1, MaxBet = 1000, MinBet = 100 }}
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
