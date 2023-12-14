namespace Windetta.Main.Core.MatchHubs.Plugins;

public interface IJoinFilter : IMatchHubPlugin
{
    internal ValueTask<(bool isValid, string? error)> ExecuteAsync(Guid userId, CancellationToken token);
}
