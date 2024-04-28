namespace Windetta.Main.Core.Lobbies.Plugins;

public abstract class JoinFilter : IJoinFilter
{
    /// <summary>
    /// Checks whether the user can join the lobby
    /// </summary>
    /// <param name="userId">Id of joining user</param>
    /// <param name="token">Cancellation token for long running processes</param>
    /// <returns> bool - result of validation, string - error message</returns>
    public abstract ValueTask<(bool isValid, string? error)> ExecuteAsync(Guid userId, CancellationToken token);
}