using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Windetta.Main.Core.Lobbies;
using Windetta.Main.Core.Lobbies.Dtos;
using Windetta.Main.Infrastructure.SignalR;

namespace Windetta.Main.Infrastructure.Lobby;

public class SignalRLobbyEventsOutput : ILobbyObserverOutput
{
    private readonly ILogger<SignalRLobbyEventsOutput> _logger;
    private readonly IHubContext<MainHub> _context;

    public SignalRLobbyEventsOutput(
        ILogger<SignalRLobbyEventsOutput> logger, 
        IHubContext<MainHub> context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task WriteLobbyAdded(ILobby lobby)
    {
        _logger.LogDebug($"Lobby added: {lobby.Id}");

        await _context.Clients.All.SendAsync("onAddedLobby", lobby.Id.ToString());
    }

    public async Task WriteLobbyDeleted(ILobby lobby)
    {
        _logger.LogDebug($"Lobby deleted: {lobby.Id}");

        await _context.Clients.All.SendAsync("onDeletedLobby", lobby.Id.ToString());
    }

    public async Task WriteLobbyReady(ILobby lobby)
    {
        _logger.LogDebug($"Lobby ready: {lobby.Id}");

        await _context.Clients.All.SendAsync("onReadyLobby", lobby.Id.ToString());
    }

    public async Task WriteLobbyUpdated(ILobby lobby)
    {
        _logger.LogDebug($"Lobby updated: {lobby.Id}");

        var dto = new LobbyDto(lobby);

        await _context.Clients.All.SendAsync("onUpdateddLobby", dto);
    }
}
