using Polly;
using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Infrastructure.Retries;

public static class PollyPipelines
{
    public static void SearchingGameServerActivity(ResiliencePipelineBuilder builder)
    {
        builder.AddRetry(new()
        {
            ShouldHandle = new PredicateBuilder()
            .Handle<LspmException>(),

            MaxRetryAttempts = 20,
            Delay = TimeSpan.FromSeconds(10)
        });
    }
}
