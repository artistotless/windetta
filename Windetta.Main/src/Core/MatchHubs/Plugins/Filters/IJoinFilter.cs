using Windetta.Main.Core.MatchHubs;

namespace Windetta.Main.MatchHubs.Filters;

internal interface IJoinFilter : IMatchHubPlugin
{
    ValueTask<(bool isValid, string? error)> ExecuteAsync(Guid userId, CancellationToken token);
}
