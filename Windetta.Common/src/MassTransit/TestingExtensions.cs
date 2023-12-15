using MassTransit;
using MassTransit.Saga;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Windetta.Common.MassTransit;

public static class TestingExtensions
{
    public static IServiceCollection ConfigureMassTransit(this IServiceCollection services, string serviceName, IUseHarness? userCfg = null)
    {
        services.AddQuartz(x =>
        {
            x.UseMicrosoftDependencyInjectionJobFactory();
        })
        .AddMassTransitTestHarness(x =>
        {
            x.SetEndpointNameFormatter(new MyEndpointNameFormatter(serviceName));

            x.AddQuartzConsumers();
            x.AddPublishMessageScheduler();

            userCfg?.ConfigureHarness().Invoke(x);

            x.UsingInMemory((context, cfg) =>
            {
                cfg.UsePublishMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static async Task AddOrUpdateSaga<TSaga>(this IServiceProvider provider, TSaga saga,
        CancellationToken cancel = default)
        where TSaga : class, ISaga
    {
        var dictionary = provider.GetRequiredService<IndexedSagaDictionary<TSaga>>();
        await dictionary.MarkInUse(cancel).ConfigureAwait(false);
        try
        {
            var existing = dictionary
                .Where(new SagaQuery<TSaga>(x => x.CorrelationId == saga.CorrelationId))
                .FirstOrDefault();
            if (existing != null) dictionary.Remove(existing);

            dictionary.Add(new SagaInstance<TSaga>(saga));
        }
        finally
        {
            dictionary.Release();
        }
    }

    public static async Task<bool> TryRemoveSaga<TSaga>(this IServiceProvider provider, Guid id,
        CancellationToken cancel = default)
        where TSaga : class, ISaga
    {
        var dictionary = provider.GetRequiredService<IndexedSagaDictionary<TSaga>>();
        await dictionary
            .MarkInUse(cancel)
            .ConfigureAwait(false);
        try
        {
            var existing = dictionary
                .Where(new SagaQuery<TSaga>(x => x.CorrelationId == id))
                .FirstOrDefault();
            if (existing == null) return false;
            dictionary.Remove(existing);

            return true;
        }
        finally
        {
            dictionary.Release();
        }
    }

    public static async Task<TInstance> GetSaga<TInstance, TStateMachine>(
        this IServiceProvider provider,
        Guid correlationId,
        TimeSpan? timeout = null,
        CancellationToken cancel = default)
        where TInstance : class, SagaStateMachineInstance
        where TStateMachine : SagaStateMachine<TInstance>
    {
        var sagaRepository = provider.GetRequiredService<ISagaRepository<TInstance>>();
        await sagaRepository
            .ShouldContainSaga(correlationId, timeout ?? TimeSpan.FromSeconds(5))
            .ConfigureAwait(false);

        var sagaHarness = provider.GetRequiredService<ISagaStateMachineTestHarness<TStateMachine, TInstance>>();
        var result = await sagaHarness.Sagas
            .SelectAsync(x => x.CorrelationId == correlationId, cancel)
            .First()
            .ConfigureAwait(false);

        return result.Saga;
    }

}
