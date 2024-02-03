namespace Windetta.Main.Core.Lobbies.Plugins;

public interface IAutoReadyStrategy : ILobbyPlugin, IDisposable
{
    void Start(ILobbyReadyListener lobby);
}
