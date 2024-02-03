using Windetta.Common.Types;

namespace Windetta.Main.Core.Lobbies;

public sealed class LobbyObserver : ISingletonService
{
    private readonly ILobbyObserverOutput _output;
    private readonly ILobbies _lobbies;

    public LobbyObserver(ILobbyObserverOutput output, ILobbies lobbies)
    {
        _output = output;
        _lobbies = lobbies;
    }

    public void AddToTracking(ILobby lobby)
    {
        lobby.Disposed += Lobby_Disposed;
        lobby.Ready += Lobby_Ready;
        lobby.Updated += Lobby_Updated;
    }

    private async void Lobby_Updated(object? sender, EventArgs e)
    {
        var lobby = (ILobby)sender!;

        await _lobbies.UpdateAsync(lobby);

        await _output.WriteLobbyUpdated(lobby);
    }

    private async void Lobby_Ready(object? sender, EventArgs e)
    {
        var lobby = (ILobby)sender!;

        await _lobbies.UpdateAsync(lobby);

        await _output.WriteLobbyReady(lobby);
    }

    private async void Lobby_Disposed(object? sender, EventArgs e)
    {
        var lobby = (ILobby)sender!;

        lobby.Ready -= Lobby_Ready;
        lobby.Updated -= Lobby_Updated;
        lobby.Disposed -= Lobby_Disposed;

        await _lobbies.RemoveAsync(lobby.Id);

        await _output.WriteLobbyDeleted(lobby);
    }
}
