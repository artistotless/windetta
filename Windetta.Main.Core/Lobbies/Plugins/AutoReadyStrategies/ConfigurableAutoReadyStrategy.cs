namespace Windetta.Main.Core.Lobbies.Plugins;

public abstract class ConfigurableAutoReadyStrategy : ConfigurableLobbyPlugin, IAutoReadyStrategy
{
    public ILobbyReadyListener Lobby { get; private set; }

    private Timer _timer;
    private readonly TimeSpan _checkInterval;

    protected ConfigurableAutoReadyStrategy(TimeSpan? checkInterval = null)
    {
        _checkInterval = checkInterval ?? TimeSpan.FromSeconds(3);
    }

    public void Start(ILobbyReadyListener lobby)
    {
        Lobby = lobby;

        if (_timer is null)
            _timer = new Timer(Update, null, TimeSpan.Zero, _checkInterval);
    }

    private void Update(object? state)
    {
        if (CheckReady())
        {
            _timer.Dispose();
            Lobby.CallReady();
        }
    }

    protected abstract bool CheckReady();

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
