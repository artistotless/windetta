using Windetta.Common.Testing;
using Windetta.Main.Core.Lobbies;

namespace Windetta.MainTests.Mocks;

public class LobbyDebugOutput : ILobbyObserverOutput
{
    private readonly XUnitOutWrapper _output;
    private readonly Queue<string> _buffer;

    public LobbyDebugOutput(XUnitOutWrapper output, ref Queue<string> buffer)
    {
        _output = output;
        _buffer = buffer;
    }

    public async Task WriteLobbyAdded(ILobby lobby)
    {
        var text = $"Lobby added: {lobby.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }

    public async Task WriteLobbyDeleted(ILobby lobby)
    {
        var text = $"Lobby deleted: {lobby.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }

    public async Task WriteLobbyReady(ILobby lobby)
    {
        var text = $"Lobby ready: {lobby.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }

    public async Task WriteLobbyUpdated(ILobby lobby)
    {
        var text = $"Lobby updated: {lobby.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }
}
