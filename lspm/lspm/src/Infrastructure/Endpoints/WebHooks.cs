using LSPM.Core.Interfaces;
using LSPM.Core.Services;
using LSPM.Infrastructure.Services;
using LSPM.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Windetta.Contracts.Events;

namespace LSPM.Infrastructure.Endpoints;

public static partial class WebHooks
{
    public static void UseWebHooks(this WebApplication web)
    {
        var hooksGroup = web.MapGroup("api/hooks");

        hooksGroup.MapPost("/matches/{id}/completed",
            async (Guid id, [FromBody] WinnersInfo info, HttpContext ctx) =>
        {
            if (!ctx.TryGetInstanceId(out Guid instanceId))
                return Results.BadRequest();

            var bus = ctx.RequestServices
            .GetRequiredService<IPublishEndpoint>();

            var store = ctx.RequestServices
            .GetRequiredService<IGameServersStore>();

            store.Find(instanceId)?.RemoveMatchWithLock(id);

            await bus.Publish<IMatchCompleted>(new
            {
                Winners = info.Winners!,
                CorrelationId = id,
            });

            return Results.NoContent();
        });

        hooksGroup.MapPost("/matches/{id}",
            async (Guid id, HttpContext ctx) =>
        {
            if (!ctx.TryGetInstanceId(out Guid instanceId))
                return Results.BadRequest();

            var bus = ctx.RequestServices
            .GetRequiredService<IPublishEndpoint>();

            await bus.Publish<IMatchCreated>(new
            {
                CorrelationId = id,
            });

            return Results.NoContent();
        });

        hooksGroup.MapPost("/matches/{id}/canceled",
            async (Guid id, [FromQuery] string reason, HttpContext ctx) =>
            {
                if (!ctx.TryGetInstanceId(out Guid instanceId))
                    return Results.BadRequest();

                var bus = ctx.RequestServices
                .GetRequiredService<IPublishEndpoint>();

                var store = ctx.RequestServices
                .GetRequiredService<IGameServersStore>();

                store.Find(instanceId)?.RemoveMatchWithLock(id);

                await bus.Publish<ICancellationMatchRequested>(new
                {
                    Reason = reason,
                    CorrelationId = id,
                });

                return Results.NoContent();
            });

        hooksGroup.MapPost("/ping", (HttpContext ctx) =>
        {
            if (!ctx.TryGetInstanceId(out Guid instanceId))
                return Results.BadRequest();

            var store = ctx.RequestServices
            .GetRequiredService<IGameServersStore>();

            var entry = store.Find(instanceId);

            if (entry is null)
                return Results.NoContent();

            entry.UpdatePingWithLock();

            return Results.NoContent();
        });

        hooksGroup.MapPost("/ready", async (HttpContext ctx, [FromServices] IGameServerMatchClient matchClient) =>
        {
            if (!ctx.TryGetInstanceId(out Guid instanceId))
                return Results.BadRequest();

            var store = ctx.RequestServices
            .GetRequiredService<IGameServersStore>();

            var bus = ctx.RequestServices
            .GetRequiredService<IPublishEndpoint>();

            var endpointResolver = ctx.RequestServices
            .GetRequiredService<EndpointResolver>();

            var ip = await endpointResolver.GetExternalIp();

            var entry = store.Find(instanceId);

            if (entry is null)
                return Results.NoContent();

            List<MatchInitializationData> datas = new();

            lock (entry.GetLock())
            {
                var currentReadyFlag = entry.IsReady;

                if (currentReadyFlag == false)
                {
                    entry.IsReady = true;

                    var queue = DelayedMatchesQueue.GetDelayedMatches(entry.InstanceId);

                    if (queue is not null)
                    {
                        while (queue.TryDequeue(out var match))
                            datas.Add(match);
                    }
                }
            }

            var createTasks = datas.Select(m => matchClient.CreateMatchAsync(entry.Endpoint, m));

            if (createTasks.Any())
                await Task.WhenAny(createTasks);

            return Results.NoContent();
        });
    }

    #region Helpers
    private static bool TryGetInstanceId(this HttpContext ctx, out Guid instanceId)
    {
        instanceId = Guid.Empty;

        if (!ctx.Request.Headers.TryGetValue("InstanceId", out var rawId) ||
        !Guid.TryParse(rawId, out instanceId))
            return false;

        return instanceId != Guid.Empty;
    }
    #endregion
}
