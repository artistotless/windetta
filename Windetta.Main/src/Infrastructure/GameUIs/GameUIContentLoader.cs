using Newtonsoft.Json;
using System.Text;
using Windetta.Common.Types;

namespace Windetta.Main.Infrastructure.GameUIs;

public static class GameUIContentLoader
{
    private const string UI_FILE_EXTENSION = "json";

    private static Dictionary<Guid, GameUIResult> _cache = new();

    private const string UIS_PATH = $"{nameof(Infrastructure)}/{nameof(GameUIs)}";

    public async static ValueTask<GameUIResult> GetUIContent(Guid gameId)
    {
        if (_cache.TryGetValue(gameId, out var cached))
            return cached;

        var path = $"{UIS_PATH}/{gameId}.{UI_FILE_EXTENSION}";
        var uiContentCache = await GetUIFileContent(path);

        if (uiContentCache is not null)
            _cache.TryAdd(gameId, uiContentCache);
        else
            throw new WindettaException($"{path} not found");

        return uiContentCache;
    }

    private static async ValueTask<GameUIResult?> GetUIFileContent(string path)
    {
        string json;

        var dir = Path.GetFullPath(path);
        using (StreamReader streamReader = new StreamReader(dir, Encoding.UTF8))
        {
            json = await streamReader.ReadToEndAsync();
        }

        var content = JsonConvert.DeserializeObject<GameUIResult>(json);

        return content;
    }
}
