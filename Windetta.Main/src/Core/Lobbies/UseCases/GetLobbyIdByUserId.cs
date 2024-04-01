namespace Windetta.Main.Core.Lobbies.UseCases;

public class GetLobbyIdByUserId : IGetLobbyIdByUserIdUseCase
{
    private readonly IUserLobbyMaps _lobbiesUsersSets;

    public GetLobbyIdByUserId(IUserLobbyMaps lobbiesUsersSets)
    {
        _lobbiesUsersSets = lobbiesUsersSets;
    }

    public Task<Guid?> ExecuteAsync(Guid userId)
    {
        return Task.FromResult(_lobbiesUsersSets.Get(userId)?.LobbyId);
    }
}