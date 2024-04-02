using Windetta.Main.Core.Clients;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Web.Api.Clients;

public sealed class LobbyEndpointsClient : ILobbyEndpointsClient
{
    private readonly LobbiesInteractor _interactor;

    public LobbyEndpointsClient(LobbiesInteractor interactor)
    {
        _interactor = interactor;
    }

    public Task<ILobby> CreateAsync(CreateLobbyDto request)
        => _interactor.CreateAsync(request);

    public Task JoinMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex)
        => _interactor.JoinMemberAsync(userId, lobbyId, roomIndex);

    public Task LeaveMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex)
        => _interactor.LeaveMemberAsync(userId, lobbyId, roomIndex);
}