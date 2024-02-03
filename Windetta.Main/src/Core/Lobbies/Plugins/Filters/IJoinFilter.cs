namespace Windetta.Main.Core.Lobbies.Plugins;

public interface IJoinFilter : ILobbyPlugin
{
    internal ValueTask<(bool isValid, string? error)> ExecuteAsync(Guid userId, CancellationToken token);
}
