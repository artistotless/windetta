namespace Windetta.Main.Core.MatchHubs.Plugins;

internal interface IJoinFilter : IMatchHubPlugin
{
    ValueTask<(bool isValid, string? error)> ExecuteAsync(Guid userId, CancellationToken token);
}
