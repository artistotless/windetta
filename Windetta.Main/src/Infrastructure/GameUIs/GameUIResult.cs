using System.Text.Json.Serialization;

namespace Windetta.Main.Infrastructure.GameUIs;

public sealed class GameUIResult
{
    public string HtmlContent { get; set; }
    public string[] Scripts { get; set; }

    public GameUIResult(string htmlContent, string[] scripts)
    {
        HtmlContent = htmlContent;
        Scripts = scripts;
    }

    [JsonConstructor]
    public GameUIResult()
    {

    }
}