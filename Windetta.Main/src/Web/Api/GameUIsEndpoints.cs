using Microsoft.AspNetCore.Mvc;
using System.Text;
using Windetta.Main.Core.Games;

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
            var uiContent = await uis.GetAsync(id);
            return Results.Content(uiContent, "text/html", Encoding.UTF8);
        });
    }
}