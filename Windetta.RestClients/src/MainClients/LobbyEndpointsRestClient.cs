using Windetta.Main.Core.Clients;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.RestClients.MainClients;

public sealed class LobbyEndpointsRestClient : ILobbyEndpointsClient
{
    private readonly HttpClient _httpClient;

    public LobbyEndpointsRestClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<ILobby> CreateAsync(CreateLobbyDto request)
    {
        throw new NotImplementedException();
    }

    public Task JoinMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex)
    {
        throw new NotImplementedException();
    }

    public Task LeaveMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex)
    {
        throw new NotImplementedException();
    }
}