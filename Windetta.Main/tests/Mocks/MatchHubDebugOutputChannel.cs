using Windetta.Common.Testing;
using Windetta.Main.MatchHub;

namespace Windetta.MainTests.Mocks;

public class MatchHubDebugOutputChannel : IMatchHubDispatcherOutputChannel
{
    private readonly XUnitOutWrapper _output;
    private readonly Queue<string> _buffer;

    public MatchHubDebugOutputChannel(XUnitOutWrapper output, ref Queue<string> buffer)
    {
        _output = output;
        _buffer = buffer;
    }

    public async Task SendHubDeleted(IMatchHub hub)
    {
        var text = $"Hub deleted: {hub.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }

    public async Task SendHubReady(IMatchHub hub)
    {
        var text = $"Hub ready: {hub.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }

    public async Task SendHubUpdated(IMatchHub hub)
    {
        var text = $"Hub updated: {hub.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }
}
