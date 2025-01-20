using LSPM.Core.Interfaces;
using LSPM.Core.Models;
using Windetta.Common.Testing;

namespace LspmTests.Mocks;

public class InMemoryGameServersOptions : IGameServersOptions
{
    private readonly IEnumerable<GameServerOptions> _options;

    public InMemoryGameServersOptions(IEnumerable<GameServerOptions>? initial = null)
    {
        if (initial is null || !initial.Any())
        {
            initial = new[] {
                new GameServerOptions()
                {
                    GameId = ExampleGuids.GameId,
                    MaxMatchesOnInstance = 2,
                    MaxInstances = 1,
                }
           };
        }

        _options = initial;
    }

    public GameServerOptions Get(Guid gameId)
    {
        return _options.First(s => s.GameId == gameId);
    }

    public static InMemoryGameServersOptions OverloadCase =>
        new InMemoryGameServersOptions(new List<GameServerOptions>()
        {
            new GameServerOptions()
            {
                GameId = ExampleGuids.GameId,
                MaxMatchesOnInstance = 0,
                MaxInstances = 0,
            }
        });
}
