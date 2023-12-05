using Windetta.Main.MatchHubs.Filters;

namespace Windetta.MainTests.Mocks;

public class AlwaysFalseJoinFilter : JoinFilter
{
    public override async ValueTask<(bool isValid, string? error)> ExecuteAsync(Guid userId, CancellationToken token)
    {
        await Task.Delay(2_000);

        return (false, "join rejected");
    }
}