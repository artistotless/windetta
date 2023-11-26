using Windetta.Common.Types;

namespace Windetta.Main.MatchHub;

public class MatchHubsDispatcher : ISingletonService
{
    private readonly IMatchHubDispatcherOutputChannel _output;

    public MatchHubsDispatcher(IMatchHubDispatcherOutputChannel output)
    {
        _output = output;
    }

    public void AddToTracking(IMatchHub hub)
    {
        hub.Disposed += Hub_Disposed;
        hub.Ready += Hub_Ready;
        hub.Updated += Hub_Updated;
    }

    private async void Hub_Updated(object? sender, EventArgs e)
    {
        var hub = (IMatchHub)sender!;

        await _output.SendHubUpdated(hub);
    }

    private async void Hub_Ready(object? sender, EventArgs e)
    {
        var hub = (IMatchHub)sender!;

        await _output.SendHubReady(hub);
    }

    private async void Hub_Disposed(object? sender, EventArgs e)
    {
        var hub = (IMatchHub)sender!;

        hub.Ready -= Hub_Ready;
        hub.Updated -= Hub_Updated;
        hub.Disposed -= Hub_Disposed;

        await _output.SendHubDeleted(hub);
    }
}
