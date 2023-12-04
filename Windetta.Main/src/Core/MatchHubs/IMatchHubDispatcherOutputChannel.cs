using Windetta.Common.Types;

namespace Windetta.Main.MatchHubs;

public interface IMatchHubDispatcherOutputChannel : ISingletonService
{
    Task WriteHubDeleted(IMatchHub hub);
    Task WriteHubReady(IMatchHub hub);
    Task WriteHubUpdated(IMatchHub hub);
}