using Windetta.Common.Testing;
using Windetta.Main.Games;

namespace Windetta.MainTests.Mocks;

public class GamesRepositoryMock : MockInitializator<IGames>
{
    private readonly GameConfiguration _gameConfiguration;
    private readonly IEnumerable<SupportedCurrency> _sc;
    private readonly InMemoryGamesRepository _gameRepository;

    public GamesRepositoryMock(
        GameConfiguration gameConfiguration,
        IEnumerable<SupportedCurrency> sc)
    {
        _gameConfiguration = gameConfiguration;
        _sc = sc;
        _gameRepository = new();
    }

    protected override void Setup(Mock<IGames> mock)
    {
        mock
            .Setup(x => x.GetConfigurationsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((_gameConfiguration, _sc));
        mock
            .Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .Returns((Guid id) => _gameRepository.GetAsync(id));
        mock
            .Setup(x => x.RemoveAsync(It.IsAny<Game>()))
            .Returns((Game game) => _gameRepository.RemoveAsync(game));
        mock
            .Setup(x => x.UpdateAsync(It.IsAny<Game>()))
            .Returns((Game game) => _gameRepository.UpdateAsync(game));
        mock
            .Setup(x => x.AddAsync(It.IsAny<Game>()))
            .Returns((Game game) => _gameRepository.AddAsync(game));
    }
}
