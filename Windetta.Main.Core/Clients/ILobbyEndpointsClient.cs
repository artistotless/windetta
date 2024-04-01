using Windetta.Common.Types;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;

namespace Windetta.Main.Clients;

public interface ILobbyEndpointsClient : IScopedService
{
    Task<ILobby> CreateAsync(CreateLobbyDto request);
    Task JoinMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex);
    Task LeaveMemberAsync(Guid userId, Guid lobbyId, ushort roomIndex);
}