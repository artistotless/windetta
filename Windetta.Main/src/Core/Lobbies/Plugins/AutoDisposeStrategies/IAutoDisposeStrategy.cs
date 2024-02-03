namespace Windetta.Main.Core.Lobbies.Plugins;

public interface IAutoDisposeStrategy : ILobbyPlugin, IDisposable
{
    void Start(ILobbyDisposeListener lobby);
}
