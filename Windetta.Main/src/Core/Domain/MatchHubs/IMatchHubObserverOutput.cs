using Windetta.Common.Types;

namespace Windetta.Main.Core.Domain.MatchHubs;

public interface IMatchHubObserverOutput : ISingletonService
{
    Task WriteHubDeleted(IMatchHub hub);
    Task WriteHubReady(IMatchHub hub);
    Task WriteHubUpdated(IMatchHub hub);
}