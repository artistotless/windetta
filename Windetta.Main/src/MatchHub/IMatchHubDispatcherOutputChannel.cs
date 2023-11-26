namespace Windetta.Main.MatchHub;

public interface IMatchHubDispatcherOutputChannel
{
    Task SendHubDeleted(IMatchHub hub);
    Task SendHubReady(IMatchHub hub);
    Task SendHubUpdated(IMatchHub hub);
}