using Windetta.Common.Testing;
using Windetta.Main.MatchHubs;

namespace Windetta.MainTests.Mocks;

public class MatchHubDebugOutput : IMatchHubObserverOutput
{
    private readonly XUnitOutWrapper _output;
    private readonly Queue<string> _buffer;

    public MatchHubDebugOutput(XUnitOutWrapper output, ref Queue<string> buffer)
    {
        _output = output;
        _buffer = buffer;
    }

    public async Task WriteHubDeleted(IMatchHub hub)
    {
        var text = $"Hub deleted: {hub.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }

    public async Task WriteHubReady(IMatchHub hub)
    {
        var text = $"Hub ready: {hub.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }

    public async Task WriteHubUpdated(IMatchHub hub)
    {
        var text = $"Hub updated: {hub.Id}";

        _buffer.Enqueue(text);
        await _output.WriteLineAsync(text);
    }
}
