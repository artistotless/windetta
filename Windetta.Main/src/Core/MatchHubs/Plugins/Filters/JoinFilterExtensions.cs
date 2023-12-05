using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.MatchHubs.Filters;

internal static class JoinFilterExtensions
{
    internal static async Task ExecuteFiltersAsync(this IEnumerable<IJoinFilter> filters, Guid userId)
    {
        if (filters.Count() == 0)
            return;

        var cancelTokenSource = new CancellationTokenSource();
        var token = cancelTokenSource.Token;

        var filterTasksQuery = filters.Select(x => x.ExecuteAsync(userId, token));

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
            var exception = MatchHubPluginException.JoinFilterValidationFail;
            exception.Message = errorMessage;

            throw exception;
        }
    }
}