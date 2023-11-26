using Windetta.Main.MatchHub.Filters;

namespace Windetta.MainTests.Mocks;

public class AlwaysFalseJoinFilter : IJoinFilter
{
    public string Name { get; init; } = "Always rejects joining member";

    public async Task<(bool, string?)> ValidateAsync(Guid userId, CancellationToken token)
    {
        await Task.Delay(2_000);

        return (false, "join rejected");
    }
}
