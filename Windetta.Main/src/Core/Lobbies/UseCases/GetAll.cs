using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Core.Lobbies.UseCases;

public class GetAll : IGetAllLobbiesUseCase
{
    private readonly ILobbies _lobbies;

    public GetAll(ILobbies lobbies)
    {
        _lobbies = lobbies;
    }

    public async Task<IEnumerable<LobbyDto>> ExecuteAsync()
    {
        return await _lobbies.GetAllAsync();
    }
}
