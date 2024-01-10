using Windetta.Common.Types;

namespace Windetta.Main.Core.Lobbies.Plugins;

public interface ILobbyPluginsFactory : IScopedService
{
    public T Get<T>(string pluginName, Dictionary<string, string>? requirementsValues) where T : ILobbyPlugin;
}