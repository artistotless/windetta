using System.Text;

namespace Windetta.Main.Infrastructure.GameUIs;

public static class GameUIContentLoader
{
    private static string? uiContentCache;

    private const string UIS_PATH = "Infrastructure/GameUIs"; // bin\Debug\net8.0\Views

    public async static ValueTask<string> GetUIContent(Guid gameId)
    {
        if (uiContentCache is not null)
            return uiContentCache;

        uiContentCache = await GetUIContent($"{UIS_PATH}/{gameId}");

        return uiContentCache;
    }

    private static async ValueTask<string> GetUIContent(string path)
    {
        string readContents;
        var dir = Path.GetFullPath(path);
        using (StreamReader streamReader = new StreamReader(dir, Encoding.UTF8))
        {
            readContents = await streamReader.ReadToEndAsync();
        }

        return readContents;
    }
}
