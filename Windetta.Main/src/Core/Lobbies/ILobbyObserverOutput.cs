using Windetta.Common.Types;

namespace Windetta.Main.Core.Lobbies;

public interface ILobbyObserverOutput : ISingletonService
{
    Task WriteLobbyDeleted(ILobby lobby);
    Task WriteLobbyReady(ILobby lobby);
    Task WriteLobbyUpdated(ILobby lobby);
}