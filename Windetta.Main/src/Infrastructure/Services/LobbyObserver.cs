using MassTransit;
using Windetta.Common.Types;
using Windetta.Contracts.Events;
using Windetta.Main.Core.Lobbies;

namespace Windetta.Main.Infrastructure.Services;

public sealed class LobbyObserver : ISingletonService
{
    private readonly ILobbyObserverOutput _output;
    private readonly IPublishEndpoint _publisher;
    private readonly ILobbies _lobbies;

    public LobbyObserver(
        ILobbies lobbies,
        ILobbyObserverOutput output,
        IPublishEndpoint publisher)
    {
        _output = output;
        _lobbies = lobbies;
        _publisher = publisher;
    }

    public void AddToTracking(ILobby lobby)
    {
        lobby.Disposed += Lobby_Disposed;
        lobby.Ready += Lobby_Ready;
        lobby.Updated += Lobby_Updated;

        _output.WriteLobbyAdded(lobby);
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

        await _publisher.Publish<ILobbyReady>(new
        {
            CorrelationId = Guid.NewGuid(),
            LobbyId = lobby.Id,
            TimeStamp = DateTime.UtcNow,
        });
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
