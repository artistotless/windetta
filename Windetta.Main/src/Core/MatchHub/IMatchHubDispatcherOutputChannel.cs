using Windetta.Common.Types;

namespace Windetta.Main.MatchHub;

public interface IMatchHubDispatcherOutputChannel : ISingletonService
{
    Task WriteHubDeleted(IMatchHub hub);
    Task WriteHubReady(IMatchHub hub);
    Task WriteHubUpdated(IMatchHub hub);
}