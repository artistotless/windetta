using Windetta.Main.Core.Exceptions;
using Windetta.Main.Core.MatchHubs;

namespace Windetta.Main.MatchHubs.Filters;

public interface IJoinFilter : IMatchHubPlugin
{
    /// <summary>
    /// Checks whether the user can join the match hub
    /// </summary>
    /// <param name="userId">Id of user</param>
    /// <param name="token">Cancellation token for long running processes</param>
    /// <returns> bool - result of validation, string - error message</returns>
    ValueTask<(bool isValid, string? error)> ValidateAsync(Guid userId, CancellationToken token);
}

public interface IJoinFilter<T> : IJoinFilter
{
    T Data { get; set; }
}

public static class JoinFilterExtensions
{
    public static async Task ExecuteFiltersAsync(this IEnumerable<IJoinFilter> filters, Guid userId)
    {
        if (filters.Count() == 0)
            return;

        var cancelTokenSource = new CancellationTokenSource();
        var token = cancelTokenSource.Token;

        var filterTasksQuery = filters.Select(x => x.ValidateAsync(userId, token));

        List<Task<(bool, string?)>> filterTasks = filterTasksQuery
            .Select(x => x.AsTask()).ToList();

        bool allowToJoin = true;
        string errorMessage = string.Empty;

        while (filterTasks.Any())
        {
            var finishedTask = await Task.WhenAny(filterTasks);
            filterTasks.Remove(finishedTask);

            (allowToJoin, errorMessage) = await finishedTask;

            if (!allowToJoin)
            {
                // cancel other filter tasks
                cancelTokenSource.Cancel();
                break;
            }
        }

        if (!allowToJoin)
        {
            var exception = MatchHubException.JoinFilterValidationFail;
            exception.Message = errorMessage;

            throw exception;
        }
    }
}