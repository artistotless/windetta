using Polly;
using Windetta.Main.Core.Exceptions;

namespace Windetta.Main.Infrastructure.Retries;

public static class PollyPipelines
{
    public static void AddSearchGameServerConsumerRetryPolicy
        (ResiliencePipelineBuilder builder)
    {
        builder.AddRetry(new()
        {
            ShouldHandle = new PredicateBuilder()
            .Handle<LspmException>(),

            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(10)
        });
    }
}
