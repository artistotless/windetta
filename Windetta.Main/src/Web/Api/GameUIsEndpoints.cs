using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Types;
using Windetta.Main.Infrastructure.GameUIs;
using Windetta.Main.Infrastructure.Services;

namespace Windetta.Main.Web.Api;

public static class GameUIsEndpoints
{
    public static void UseGameUIsEndpoints(this WebApplication web)
    {
        // Get current match information
        web.MapGet("api/gameuis/{id:Guid}", async (
            [FromRoute] Guid id,
            [FromServices] IGameUIs uis) =>
        {
            var content = await uis.GetAsync(id);
            var response = new BaseResponse<GameUIResult>(content);

            return Results.Json(response);
        });
    }
}