namespace Windetta.Main.Core.Lobbies.UseCases;

public class GetLobbyIdByUserId : IGetLobbyIdByUserIdUseCase
{
    private readonly ILobbyUsersAssociations _lobbiesUsersSets;

    public GetLobbyIdByUserId(ILobbyUsersAssociations lobbiesUsersSets)
    {
        _lobbiesUsersSets = lobbiesUsersSets;
    }

    public Task<Guid?> ExecuteAsync(Guid userId)
    {
        return Task.FromResult(_lobbiesUsersSets.GetLobbyId(userId));
    }
}