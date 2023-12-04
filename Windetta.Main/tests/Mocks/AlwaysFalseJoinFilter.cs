using Windetta.Main.MatchHubs.Filters;

namespace Windetta.MainTests.Mocks;

public class AlwaysFalseJoinFilter : IJoinFilter
{
    public async ValueTask<(bool, string?)> ValidateAsync(Guid userId, CancellationToken token)
    {
        await Task.Delay(2_000);

        return (false, "join rejected");
    }
}