namespace Windetta.Main.MatchHub.Filters;

public interface IJoinFilter
{
    string Name { get; init; }

    /// <summary>
    /// Checks whether the user can join the match hub
    /// </summary>
    /// <param name="userId">Id of user</param>
    /// <param name="token">Cancellation token for long running processes</param>
    /// <returns> bool - result of validation, string - error message</returns>
    Task<(bool, string?)> ValidateAsync(Guid userId, CancellationToken token);
}
