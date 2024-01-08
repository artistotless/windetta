using Windetta.Common.Types;

namespace Windetta.Main.Core.MatchHubs;

public sealed class MatchHubObserver : ISingletonService
{
    private readonly IMatchHubObserverOutput _output;
    private readonly IMatchHubs _hubs;

    public MatchHubObserver(IMatchHubObserverOutput output, IMatchHubs hubs)
    {
        _output = output;
        _hubs = hubs;
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

        await _hubs.UpdateAsync(hub);

        await _output.WriteHubUpdated(hub);
    }

    private async void Hub_Ready(object? sender, EventArgs e)
    {
        var hub = (IMatchHub)sender!;

        await _hubs.UpdateAsync(hub);

        await _output.WriteHubReady(hub);
    }

    private async void Hub_Disposed(object? sender, EventArgs e)
    {
        var hub = (IMatchHub)sender!;

        hub.Ready -= Hub_Ready;
        hub.Updated -= Hub_Updated;
        hub.Disposed -= Hub_Disposed;

        await _hubs.RemoveAsync(hub.Id);

        await _output.WriteHubDeleted(hub);
    }
}
