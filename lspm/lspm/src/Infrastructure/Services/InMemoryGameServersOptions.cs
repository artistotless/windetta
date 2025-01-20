using LSPM.Core.Interfaces;
using LSPM.Core.Models;

namespace LSPM.Infrastructure.Services;

public class InMemoryGameServersOptions : IGameServersOptions
{
    private readonly IEnumerable<GameServerOptions> _options;

    public InMemoryGameServersOptions(IConfiguration configuration)
    {
        var values = configuration
            .GetSection(nameof(GameServerOptions))
            .Get<IEnumerable<GameServerOptions>>();

        _options = values ?? new List<GameServerOptions>();
    }

    public GameServerOptions Get(Guid gameId)
    {
        return _options.First(s => s.GameId == gameId);
    }
}
